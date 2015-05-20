// The MIT License
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
using FluentAssertions;
using NUnit.Framework;
using iSynaptic.Commons;
using iSynaptic.Serialization;
using iSynaptic.TestDomain;

namespace iSynaptic.Modeling.Domain
{
    [TestFixture]
    public class AggregateMementoTests
    {
        [Test]
        public void Class_IsJsonSerializable()
        {
            var serviceCase = new ServiceCase(
                ServiceCase.SampleContent.Title, 
                ServiceCase.SampleContent.Description, 
                ServiceCasePriority.Normal,
                ServiceCase.SampleContent.ResponsibleParty);

            var thread = serviceCase.StartCommunicationThread(
                ServiceCase.SampleContent.Topic, 
                ServiceCase.SampleContent.TopicDescription,
                ServiceCase.SampleContent.ResponsibleParty);

            thread.RecordCommunication(
                CommunicationDirection.Incoming, 
                ServiceCase.SampleContent.CommunicationContent,
                SystemClock.UtcNow,
                ServiceCase.SampleContent.CommunicationDuration,
                ServiceCase.SampleContent.ResponsibleParty);

            var memento = new AggregateMemento(
                typeof (ServiceCase),
                serviceCase.TakeSnapshot().ToMaybe(),
                serviceCase.GetEvents());

            var serializer = JsonSerializerBuilder.Build(LogicalTypeRegistryBuilder.Build());

            String json = serializer.Serialize(memento);
            var reconstituted = serializer.Deserialize<AggregateMemento>(json);

            reconstituted.Should().NotBeNull();
            reconstituted.Should().NotBeSameAs(memento);
        }
    }
}
