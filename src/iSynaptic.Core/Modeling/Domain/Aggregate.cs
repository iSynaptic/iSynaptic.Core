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
using iSynaptic.Commons.Linq;

namespace iSynaptic.Modeling.Domain
{
    internal interface IAggregateInternal<out TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        void Initialize(IAggregateMemento memento);

        void ApplyEvents(IEnumerable<IAggregateEvent> events);
        IEnumerable<IAggregateEvent<TIdentifier>> GetUncommittedEvents();

        Boolean ConflictsWith(IEnumerable<IAggregateEvent> committedEvents, IEnumerable<IAggregateEvent> attemptedEvents);

        void CommitEvents();
    }
    
    public abstract class Aggregate<TIdentifier> : IAggregate<TIdentifier>, IAggregateInternal<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        [ImmuneToReset]
        private AggregateEventStream<TIdentifier> _events;

        [ImmuneToReset]
        private Action<IAggregate<TIdentifier>> _resetOperation;

        [ImmuneToReset]
        private Action<IAggregate<TIdentifier>, IAggregateEvent<TIdentifier>> _eventDispatcher;

        [ImmuneToReset]
        private Action<IAggregate<TIdentifier>, IAggregateSnapshot<TIdentifier>> _snapshotDispatcher;

        protected Aggregate()
        {
            Initialize(null);
        }

        private void Initialize(IAggregateMemento memento)
        {
            _events = new AggregateEventStream<TIdentifier>();

            if (_resetOperation == null)
                _resetOperation = AggregateHelper.GetResetOperation<TIdentifier>(GetType());

            _resetOperation(this);
            OnInitialize();

            if (memento != null)
            {
                var m = memento.ToMemento<TIdentifier>();

                if (m.Snapshot.HasValue)
                    ApplySnapshot(m.Snapshot.Value);

                ApplyEventsCore(m.Events);
                _events.CommitEvents();
            }
        }

        void IAggregateInternal<TIdentifier>.Initialize(IAggregateMemento memento)
        {
            Initialize(memento.ToMemento<TIdentifier>());
        }

        private void ApplySnapshot(IAggregateSnapshot<TIdentifier> snapshot)
        {
            Id = snapshot.Id;
            Version = snapshot.Version;

            DispatchSnapshot(snapshot);
        }

        protected void ApplyEvent(Func<TIdentifier, Int32, IAggregateEvent<TIdentifier>> eventFactory)
        {
            if(Version <= 0)
                throw new InvalidOperationException("This overload of ApplyEvent can only be called after the first event is applied.");

            ApplyEvent(eventFactory(Id, Version + 1));
        }

        protected void ApplyEvent(IAggregateEvent<TIdentifier> @event)
        {
            Guard.NotNull(@event, "event");

            if (_events == null)
                throw new InvalidOperationException("Events cannot be applied until aggregate is initialized. Ensure base constructor is called.");

            ApplyEventsCore(new[] { @event });
        }

        void IAggregateInternal<TIdentifier>.ApplyEvents(IEnumerable<IAggregateEvent> events)
        {
            ApplyEventsCore(events.Cast<IAggregateEvent<TIdentifier>>());
        }

        internal void ApplyEventsCore(IEnumerable<IAggregateEvent<TIdentifier>> events)
        {
            lock (_events)
            {
                foreach (var @event in events)
                {
                    if (_events.Version <= 0)
                        Id = @event.Id;

                    Version = @event.Version;

                    DispatchEvent(@event);
                    _events.AppendEvent(@event);
                }
            }
        }

        protected virtual void DispatchEvent(IAggregateEvent<TIdentifier> @event)
        {
            if(_eventDispatcher == null)
                _eventDispatcher = AggregateHelper.GetEventDispatcher<TIdentifier>(GetType());

            _eventDispatcher(this, @event);
        }

        protected virtual void DispatchSnapshot(IAggregateSnapshot<TIdentifier> snapshot)
        {
            if(_snapshotDispatcher == null)
                _snapshotDispatcher = AggregateHelper.GetSnapshotDispatcher<TIdentifier>(GetType());

            _snapshotDispatcher(this, snapshot);
        }

        protected virtual void OnInitialize() { }
        public virtual IAggregateSnapshot<TIdentifier> TakeSnapshot() { return null; }
        public IEnumerable<IAggregateEvent<TIdentifier>> GetEvents() { return _events.Events; }
            
        IEnumerable<IAggregateEvent<TIdentifier>> IAggregateInternal<TIdentifier>.GetUncommittedEvents()
        {
            return _events.UncommittedEvents;
        }

        void IAggregateInternal<TIdentifier>.CommitEvents()
        {
            _events.CommitEvents();
        }

        Boolean IAggregateInternal<TIdentifier>.ConflictsWith(IEnumerable<IAggregateEvent> committedEvents,
                                                              IEnumerable<IAggregateEvent> attemptedEvents)
        {
            return ConflictsWith(committedEvents.Cast<IAggregateEvent<TIdentifier>>(),
                                 attemptedEvents.Cast<IAggregateEvent<TIdentifier>>());
        }

        protected internal virtual Boolean ConflictsWith(IEnumerable<IAggregateEvent<TIdentifier>> committedEvents, IEnumerable<IAggregateEvent<TIdentifier>> attemptedEvents)
        {
            var conflicts = from committedEvent in committedEvents
                            from attemptedEvent in attemptedEvents
                            select ConflictsWith(committedEvent, attemptedEvent);

            return !conflicts.AllFalse();
        }

        protected internal virtual Boolean ConflictsWith(IAggregateEvent<TIdentifier> committedEvent, IAggregateEvent<TIdentifier> attemptedEvent) { return true; }
        
        public TIdentifier Id { get; private set; }
        public Int32 Version { get; private set; }
    }
}