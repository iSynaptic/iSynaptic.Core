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
using System.Threading.Tasks;
using iSynaptic.Commons;
using iSynaptic.Modeling.Domain;

namespace iSynaptic.TestDomain
{
    public class ServiceCaseProjector : MessageHandler
    {
        private readonly List<CommThread> _threads = new List<CommThread>();

        public async Task HandleEvents(IEnumerable<IAggregateEvent> events)
        {
            foreach (var @event in events)
                await OnHandle(@event);
        }

        protected override bool ShouldHandle(Object message)
        {
            return message.GetType() != typeof (IgnoredEvent);
        }

        private void On(ServiceCase.Opened @event)
        {
            Title = @event.Title;
            Description = @event.Description;
            Priority = @event.Priority;
        }

        private Task On(ServiceCase.CommunicationThreadStarted @event)
        {
            return Task.Run(() => _threads.Add(new CommThread(@event.ThreadId, @event.Topic, @event.Description)));
        }

        private void On(ServiceCase.CommunicationRecorded @event)
        {
            var thread = _threads.First(x => x.ThreadId == @event.ThreadId);
            thread.Add(new Communication(@event.Direction, @event.Content, @event.CommunicationTime));
        }

        public string Title { get; private set; }
        public string Description { get; private set; }
        public ServiceCase.Priority? Priority { get; private set; }

        public IEnumerable<CommThread> Threads { get { return _threads; } }

        public class CommThread
        {
            private readonly List<Communication> _communications = new List<Communication>();

            public CommThread(Int32 threadId, string topic, string description)
            {
                ThreadId = threadId;
                Topic = topic;
                Description = description;
            }

            public void Add(Communication communication)
            {
                _communications.Add(communication);
            }

            public Int32 ThreadId { get; private set; }
            public String Topic { get; private set; }
            public String Description { get; private set; }

            public IEnumerable<Communication> Communications { get { return _communications; } }
        }

        public class Communication
        {
            public Communication(CommunicationDirection direction, String content, DateTime communicationTime)
            {
                Direction = direction;
                Content = content;
                CommunicationTime = communicationTime;
            }

            public CommunicationDirection Direction { get; private set; }
            public String Content { get; private set; }
            public DateTime CommunicationTime { get; private set; }
        }
    }
}
