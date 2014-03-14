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
using System.Collections.Generic;
using System.Linq;
using iSynaptic.Commons;
using iSynaptic.Modeling.Domain;

namespace iSynaptic.TestDomain
{
    // This is a poor example of an aggregate since it is devoid of much logic and exposes state.
    // It exists only to facilitate testing of the framework around aggregates
    public partial class ServiceCase
    {
        public static class SampleContent
        {
            public const String Title = "Tablet battery died";
            public const String Description = "Customer wants new battery since it died after 127 days.";
            public const String Topic = "Warranty lapsed";
            public const String TopicDescription = "Warranty no longer covers battery since the warranty expires after 90 days.";
            public const String CommunicationContent = "I can't believe your not going to replace this battery!!!";
            public static readonly TimeSpan CommunicationDuration = TimeSpan.FromMinutes(20);
        }

        private Int32 _lastThreadId;
        private List<CommunicationThread> _threads;

        #region Entities

        private class CommunicationThread : ICommunicationThread
        {
            private readonly ServiceCase _serviceCase;

            public CommunicationThread(ServiceCase serviceCase, Int32 threadId, String topic, String description)
            {
                _serviceCase = serviceCase;
                ThreadId = threadId;
                Topic = topic;
                Description = description;
            }

            public void RecordCommunication(CommunicationDirection direction, String content, DateTime communicationTime, TimeSpan duration)
            {
                _serviceCase.ApplyEvent((id, ver) => new CommunicationRecorded(ThreadId, direction, content, communicationTime, duration, id, ver));
            }

            // these exists ONLY for testing purposes; aggregates should not expose state
            public Int32 ThreadId { get; private set; }
            public String Topic { get; private set; }
            public String Description { get; private set; }
        }

        #endregion

        public ServiceCase(String title, String description, ServiceCasePriority priority)
            : this(new Opened(title, description, priority, Guid.NewGuid(), 1))
        {
        }

        protected override void OnInitialize()
        {
            _lastThreadId = 0;
            _threads = new List<CommunicationThread>();
        }

        public override IAggregateSnapshot<ServiceCaseId> TakeSnapshot()
        {
            return new Snapshot(_lastThreadId,
                                _threads.Select(x => new CommunicationThreadSnapshot(x.ThreadId, x.Topic, x.Description)),
                                Title,
                                Description,
                                Priority,
                                Id,
                                Version,
                                SystemClock.UtcNow);
        }

        private void Apply(Snapshot snapshot)
        {
            _lastThreadId = snapshot.LastThreadId;
            _threads = snapshot.ThreadSnapshots.Select(x => new CommunicationThread(this, x.ThreadId, x.Topic, x.Description)).ToList();
            Title = snapshot.Title;
            Description = snapshot.Description;
            Priority = snapshot.Priority;
        }

        #region Commands

        public virtual ICommunicationThread StartCommunicationThread(String topic, String description)
        {
            Int32 newThreadId = _lastThreadId + 1;

            ApplyEvent((id, ver) => new CommunicationThreadStarted(newThreadId, topic, description, id, ver));

            return _threads.Single(x => x.ThreadId == newThreadId);
        }

        #endregion

        #region Event Applicators

        private void On(Opened @event)
        {
            Title = @event.Title;
            Description = @event.Description;
            Priority = @event.Priority;
        }

        private void On(CommunicationThreadStarted @event)
        {
            _lastThreadId = @event.ThreadId;

            var thread = new CommunicationThread(this, @event.ThreadId, @event.Topic, @event.Description);
            _threads.Add(thread);
        }

        protected override bool ConflictsWith(IEnumerable<IAggregateEvent<ServiceCaseId>> committedEvents, IEnumerable<IAggregateEvent<ServiceCaseId>> attemptedEvents)
        {
            return !committedEvents.All(x => x is CommunicationRecorded) &&
                   !attemptedEvents.All(x => x is CommunicationRecorded);
        }

        #endregion

        public IEnumerable<ICommunicationThread> Threads { get { return _threads; } }

        // these exists ONLY for testing purposes; aggregates should not expose state
        public String Title { get; private set; }
        public String Description { get; private set; }
        public ServiceCasePriority Priority { get; private set; }
    }

    public interface ICommunicationThread
    {
        void RecordCommunication(CommunicationDirection direction, String content, DateTime communicationTime, TimeSpan duration);

        // these exists ONLY for testing purposes; aggregates should not expose state
        Int32 ThreadId { get; }
        String Topic { get; }
        String Description { get; }
    }
}