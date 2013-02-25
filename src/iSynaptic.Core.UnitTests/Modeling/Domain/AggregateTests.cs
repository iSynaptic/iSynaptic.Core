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
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCasePriority.Normal);
            
            serviceCase.Id.Should().NotBe(Guid.Empty);
            serviceCase.Version.Should().Be(1);
            serviceCase.Title.Should().Be(ServiceCase.SampleContent.Title);
            serviceCase.Description.Should().Be(ServiceCase.SampleContent.Description);
            serviceCase.Priority.Should().Be(ServiceCasePriority.Normal);
        }

        [Test]
        public void StartCommunicationThead_YieldsEventAndReturnsThread()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCasePriority.Normal);
            var thread = serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic, ServiceCase.SampleContent.TopicDescription);

            thread.Should().NotBeNull();
            serviceCase.Threads.Any(x => x.ThreadId == thread.ThreadId).Should().BeTrue();
        }

        [Test]
        public void UncommittedEventsRetreivable()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCasePriority.Normal);
            var thread = serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic, ServiceCase.SampleContent.TopicDescription);

            thread.RecordCommunication(CommunicationDirection.Incoming, ServiceCase.SampleContent.CommunicationContent, SystemClock.UtcNow);

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
    }
}