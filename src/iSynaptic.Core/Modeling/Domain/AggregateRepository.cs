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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using iSynaptic.Commons;
using iSynaptic.Commons.Threading.Tasks;

namespace iSynaptic.Modeling.Domain
{
    public abstract class AggregateRepository<TAggregate, TIdentifier> : IAggregateRepository<TAggregate, TIdentifier> 
        where TAggregate : class, IAggregate<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        ITask<TAggregate> IAggregateRepositoryQueries<TAggregate, TIdentifier>.Get(TIdentifier id, Int32 maxVersion)
        {
            return Get(id, maxVersion).ToCovariantTask();
        }

        public async Task<TAggregate> Get(TIdentifier id, Int32 maxVersion)
        {
            var memento = await GetMemento(id, maxVersion);

            if (memento == null)
                return null;

            var aggregate = (TAggregate)FormatterServices.GetSafeUninitializedObject(memento.AggregateType);
            var ag = AsInternal(aggregate);
            ag.Initialize(memento);

            return aggregate;
        }

        public async Task Save(TAggregate aggregate)
        {
            Guard.NotNull(aggregate, "aggregate");

            var ag = AsInternal(aggregate);
            var aggregateType = aggregate.GetType();
            var events = ag.GetUncommittedEvents().ToArray();

            if (events.Length <= 0)
                return;

            var data = new AggregateEventsFrame<TIdentifier>(aggregateType, aggregate.Id, events);

            int saveAttempts = 0;
            while(true)
            {
                AggregateConcurrencyException originalException;
                try
                {
                    saveAttempts++;

                    await SaveEvents(data);
                    break;
                }
                catch (AggregateConcurrencyException ace)
                {
                    if (saveAttempts == 3)
                        throw;

                    originalException = ace;
                }
                
                var committedEvents = await GetEvents(aggregate.Id, events[0].Version, Int32.MaxValue);
                if (ag.ConflictsWith(committedEvents.Events, events))
                {
                    throw new AggregateConcurrencyException(originalException.Message,
                                                            originalException);
                }

                var newExpectedVersion = committedEvents.Events.Last().Version;

                var memento = await GetMemento(aggregate.Id, newExpectedVersion);
                ag.Initialize(memento);

                for (int i = 1; i <= events.Length; i++)
                    ((IAggregateEventInternal)events[i - 1]).Version = newExpectedVersion + i;

                ag.ApplyEvents(events);
            }

            ag.CommitEvents();
            OnEventStreamSaved(aggregateType, aggregate.Id, events);
        }

        public async Task SaveSnapshot(TAggregate aggregate)
        {
            Guard.NotNull(aggregate, "aggregate");

            var snapshot = aggregate.TakeSnapshot();
            if (snapshot == null)
                throw new ArgumentException("Aggregate doesn't support snapshots.", "aggregate");

            var aggregateType = aggregate.GetType();

            var data = new AggregateSnapshotFrame<TIdentifier>(aggregateType, aggregate.Id, snapshot);
            await SaveSnapshot(data);

            OnSnapshotSaved(aggregateType, snapshot);
        }

        private IAggregateInternal<TIdentifier> AsInternal(TAggregate aggregate)
        {
            var ag = aggregate as IAggregateInternal<TIdentifier>;
            if(ag == null) throw new InvalidOperationException("Aggregate must inherit from Aggregate<TIdentifier>.");
            return ag;
        }

        protected virtual void OnEventStreamSaved(Type aggregateType, TIdentifier id, IEnumerable<IAggregateEvent<TIdentifier>> events) { }
        protected virtual void OnSnapshotSaved(Type aggregateType, IAggregateSnapshot<TIdentifier> snapshot) { }

        public async Task<AggregateMemento<TIdentifier>> GetMemento(TIdentifier id, Int32 maxVersion)
        {
            var snapshotFrame = await GetSnapshot(id, maxVersion);
            if (snapshotFrame != null)
            {
                var snapshot = snapshotFrame.Snapshot;
                var events = Enumerable.Empty<IAggregateEvent<TIdentifier>>();
                
                if (snapshot.Version < maxVersion)
                {
                    var eventsFrame = await GetEvents(id, snapshot.Version + 1, maxVersion);
                    if(eventsFrame != null)
                        events = eventsFrame.Events;
                }

                return new AggregateMemento<TIdentifier>(snapshotFrame.AggregateType, snapshot.ToMaybe(), events);
            }
            else
            {
                var eventsFrame = await GetEvents(id, 1, maxVersion);

                return eventsFrame != null 
                    ? new AggregateMemento<TIdentifier>(eventsFrame.AggregateType, Maybe.NoValue, eventsFrame.Events)
                    : null;
            }
        }

        protected abstract Task<AggregateSnapshotFrame<TIdentifier>> GetSnapshot(TIdentifier id, Int32 maxVersion);
        protected abstract Task<AggregateEventsFrame<TIdentifier>> GetEvents(TIdentifier id, Int32 minVersion, Int32 maxVersion);

        protected abstract Task SaveSnapshot(AggregateSnapshotFrame<TIdentifier> frame);
        protected abstract Task SaveEvents(AggregateEventsFrame<TIdentifier> frame);
    }
}