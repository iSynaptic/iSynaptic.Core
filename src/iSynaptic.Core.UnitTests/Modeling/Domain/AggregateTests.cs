﻿// The MIT License
// 
// Copyright (c) 2013 Jordan E. Terrell
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using iSynaptic.Commons;
using iSynaptic.TestDomain;

namespace iSynaptic.Modeling.Domain
{
    [TestFixture]
    public class AggregateTests
    {
        [Test]
        public void CreatedServiceCase_AppliesEvent()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCase.Priority.Normal, ServiceCase.SampleContent.ResponsibleParty);
            
            serviceCase.Id.Should().NotBe(Guid.Empty);
            serviceCase.Version.Should().Be(1);
            serviceCase.Title.Should().Be(ServiceCase.SampleContent.Title);
            serviceCase.Description.Should().Be(ServiceCase.SampleContent.Description);
            serviceCase.ServiceCasePriority.Should().Be(ServiceCase.Priority.Normal);
        }

        [Test]
        public void StartCommunicationThead_YieldsEventAndReturnsThread()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCase.Priority.Normal, ServiceCase.SampleContent.ResponsibleParty);
            var thread = serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic, ServiceCase.SampleContent.TopicDescription, ServiceCase.SampleContent.ResponsibleParty);

            thread.Should().NotBeNull();
            serviceCase.Threads.Any(x => x.ThreadId == thread.ThreadId).Should().BeTrue();
        }

        [Test]
        public void UncommittedEventsRetreivable()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCase.Priority.Normal, ServiceCase.SampleContent.ResponsibleParty);
            var thread = serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic, ServiceCase.SampleContent.TopicDescription, ServiceCase.SampleContent.ResponsibleParty);

            thread.RecordCommunication(CommunicationDirection.Incoming, ServiceCase.SampleContent.CommunicationContent, SystemClock.UtcNow, ServiceCase.SampleContent.CommunicationDuration, ServiceCase.SampleContent.ResponsibleParty);

            serviceCase.GetEvents().Count().Should().Be(3);
            serviceCase.GetEvents()
                       .Select(x => x.GetType())
                       .SequenceEqual(new[]
                       {
                           typeof (ServiceCase.Opened), typeof (ServiceCase.CommunicationThreadStarted),
                           typeof (ServiceCase.CommunicationRecorded)
                       })
                       .Should().BeTrue();
        }

        [Test]
        public void Aggregate_IsMockable()
        {
            Aggregate.FieldsImmuneToReset(f => f.FieldType.FullName.Contains("DynamicProxy") || f.FieldType.FullName.Contains("NSubstitute"));

            var serviceCase = Substitute.For<ServiceCase>(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCase.Priority.Normal, ServiceCase.SampleContent.ResponsibleParty);

            serviceCase.When(x => x.StartCommunicationThread(null, null, ServiceCase.SampleContent.ResponsibleParty))
                .Do(x => { throw new Exception("bad mojo!"); });

            serviceCase.Invoking(x => x.StartCommunicationThread(null, null, ServiceCase.SampleContent.ResponsibleParty))
                .ShouldThrow<Exception>().WithMessage("bad mojo!");
        }
    }
}