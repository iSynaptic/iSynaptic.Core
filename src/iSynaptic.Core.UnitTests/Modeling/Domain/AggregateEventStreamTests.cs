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
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using iSynaptic.Commons;
using iSynaptic.TestDomain;

namespace iSynaptic.Modeling.Domain
{
    [TestFixture]
    public class AggregateEventStreamTests
    {
        private static readonly Guid _id = Guid.NewGuid();

        private static readonly ServiceCase.Opened _openedEvent
            = new ServiceCase.Opened(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCasePriority.Normal, _id, 1);


        [Test]
        public void Stream_DefaultsCorrect()
        {
            var stream = new AggregateEventStream<Guid>();
            stream.IsTruncated.Should().BeFalse();
            stream.Version.Should().Be(0);
            stream.CommittedVersion.Should().Be(0);
        }

        [Test]
        public void Appending_OutOfOrder_ThrowsException()
        {
            var stream = new AggregateEventStream<ServiceCaseId>();
            var e2 = new ServiceCase.CommunicationThreadStarted(1, ServiceCase.SampleContent.Topic, ServiceCase.SampleContent.TopicDescription, _id, 2);

            stream.AppendEvent(e2);

            stream.Invoking(x => x.AppendEvent(_openedEvent))
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void Appending_WithGaps_ThrowsException()
        {
            var stream = new AggregateEventStream<ServiceCaseId>();
            var outOfOrderEvent = new ServiceCase.CommunicationThreadStarted(1, ServiceCase.SampleContent.Topic, ServiceCase.SampleContent.TopicDescription, _id, 3);

            stream.AppendEvent(_openedEvent);

            stream.Invoking(x => x.AppendEvent(outOfOrderEvent))
                  .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void AppendingInitialEvent_WithVersionEqualToOne_IsNotTruncated()
        {
            var stream = new AggregateEventStream<ServiceCaseId>();
            stream.AppendEvent(_openedEvent);
            stream.IsTruncated.Should().BeFalse();
        }

        [Test]
        public void AppendingInitialEvent_WithVersionGreaterThanOne_IsTruncated()
        {
            var stream = new AggregateEventStream<ServiceCaseId>();
            stream.AppendEvent(new ServiceCase.CommunicationThreadStarted(1, ServiceCase.SampleContent.Topic, ServiceCase.SampleContent.TopicDescription, _id, 7));
            stream.IsTruncated.Should().BeTrue();
        }


        [Test]
        public void AppendingInitialEvent_EventStreamsAndVersionCorrect()
        {
            var stream = new AggregateEventStream<ServiceCaseId>();
            stream.AppendEvent(_openedEvent);

            stream.Events.Count().Should().Be(1);
            stream.UncommittedEvents.Count().Should().Be(1);
            stream.CommittedEvents.Count().Should().Be(0);

            stream.CommittedVersion.Should().Be(0);
            stream.Version.Should().Be(1);

            stream.UncommittedEvents.First().Should().Be(_openedEvent);
        }

        [Test]
        public void CommittingInitialEvent_EventStreamsAndVersionCorrect()
        {
            var stream = new AggregateEventStream<ServiceCaseId>();
            stream.AppendEvent(_openedEvent);
            stream.CommitEvents();

            stream.Events.Count().Should().Be(1);
            stream.UncommittedEvents.Count().Should().Be(0);
            stream.CommittedEvents.Count().Should().Be(1);

            stream.CommittedVersion.Should().Be(1);
            stream.Version.Should().Be(1);

            stream.CommittedEvents.First().Should().Be(_openedEvent);
        }

        [Test]
        public void AppendingManyEvents_EventStreamsAndVersionCorrect()
        {
            var stream = new AggregateEventStream<ServiceCaseId>();

            var e2 = new ServiceCase.CommunicationThreadStarted(1, ServiceCase.SampleContent.Topic, ServiceCase.SampleContent.TopicDescription, _id, 2);
            var e3 = new ServiceCase.CommunicationRecorded(1, CommunicationDirection.Incoming, ServiceCase.SampleContent.CommunicationContent, SystemClock.UtcNow, ServiceCase.SampleContent.CommunicationDuration, _id, 3);

            stream.AppendEvent(_openedEvent);
            stream.AppendEvent(e2);
            stream.AppendEvent(e3);

            stream.Events.Count().Should().Be(3);
            stream.UncommittedEvents.Count().Should().Be(3);
            stream.CommittedEvents.Count().Should().Be(0);

            stream.CommittedVersion.Should().Be(0);
            stream.Version.Should().Be(3);

            stream.UncommittedEvents.ElementAt(0).Should().Be(_openedEvent);
            stream.UncommittedEvents.ElementAt(1).Should().Be(e2);
            stream.UncommittedEvents.ElementAt(2).Should().Be(e3);
        }

        [Test]
        public void CommittingManyEvents_EventStreamsAndVersionCorrect()
        {
            var stream = new AggregateEventStream<ServiceCaseId>();

            var e2 = new ServiceCase.CommunicationThreadStarted(1, ServiceCase.SampleContent.Topic, ServiceCase.SampleContent.TopicDescription, _id, 2);
            var e3 = new ServiceCase.CommunicationRecorded(1, CommunicationDirection.Incoming, ServiceCase.SampleContent.CommunicationContent, SystemClock.UtcNow, ServiceCase.SampleContent.CommunicationDuration, _id, 3);

            stream.AppendEvent(_openedEvent);
            stream.AppendEvent(e2);
            stream.AppendEvent(e3);

            stream.CommitEvents();

            stream.Events.Count().Should().Be(3);
            stream.UncommittedEvents.Count().Should().Be(0);
            stream.CommittedEvents.Count().Should().Be(3);

            stream.CommittedVersion.Should().Be(3);
            stream.Version.Should().Be(3);

            stream.CommittedEvents.ElementAt(0).Should().Be(_openedEvent);
            stream.CommittedEvents.ElementAt(1).Should().Be(e2);
            stream.CommittedEvents.ElementAt(2).Should().Be(e3);
        }

        [Test]
        public void CommittingEvents_UpToVersion_EventStreamsAndVersionCorrect()
        {
            var stream = new AggregateEventStream<ServiceCaseId>();

            var e2 = new ServiceCase.CommunicationThreadStarted(1, ServiceCase.SampleContent.Topic, ServiceCase.SampleContent.TopicDescription, _id, 2);
            var e3 = new ServiceCase.CommunicationRecorded(1, CommunicationDirection.Incoming, ServiceCase.SampleContent.CommunicationContent, SystemClock.UtcNow, ServiceCase.SampleContent.CommunicationDuration, _id, 3);

            stream.AppendEvent(_openedEvent);
            stream.AppendEvent(e2);
            stream.AppendEvent(e3);

            stream.CommitEvents(2);

            stream.Events.Count().Should().Be(3);
            stream.UncommittedEvents.Count().Should().Be(1);
            stream.CommittedEvents.Count().Should().Be(2);

            stream.CommittedVersion.Should().Be(2);
            stream.Version.Should().Be(3);

            stream.CommittedEvents.ElementAt(0).Should().Be(_openedEvent);
            stream.CommittedEvents.ElementAt(1).Should().Be(e2);
            stream.UncommittedEvents.ElementAt(0).Should().Be(e3);
        }

        [Test]
        public void CommittingEvents_UpToVersion_WhenStreamIsTruncated_YieldsSomeCommittedEvents()
        {
            var stream = new AggregateEventStream<ServiceCaseId>();

            var e1 = new ServiceCase.CommunicationThreadStarted(1, ServiceCase.SampleContent.Topic, ServiceCase.SampleContent.TopicDescription, _id, 7);
            var e2 = new ServiceCase.CommunicationRecorded(1, CommunicationDirection.Incoming, ServiceCase.SampleContent.CommunicationContent, SystemClock.UtcNow, ServiceCase.SampleContent.CommunicationDuration, _id, 8);
            var e3 = new ServiceCase.CommunicationRecorded(1, CommunicationDirection.Incoming, ServiceCase.SampleContent.CommunicationContent, SystemClock.UtcNow, ServiceCase.SampleContent.CommunicationDuration, _id, 9);

            stream.AppendEvent(e1);
            stream.AppendEvent(e2);
            stream.AppendEvent(e3);

            stream.CommitEvents(8);

            stream.Events.Count().Should().Be(3);
            stream.UncommittedEvents.Count().Should().Be(1);
            stream.CommittedEvents.Count().Should().Be(2);

            stream.CommittedVersion.Should().Be(8);
            stream.Version.Should().Be(9);

            stream.CommittedEvents.ElementAt(0).Should().Be(e1);
            stream.CommittedEvents.ElementAt(1).Should().Be(e2);
            stream.UncommittedEvents.ElementAt(0).Should().Be(e3);
        }
    }
}