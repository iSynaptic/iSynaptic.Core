// The MT License
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
using System.Runtime.Serialization;
using System.Threading.Tasks;
using iSynaptic.Commons;
using iSynaptic.Commons.Reflection;
using iSynaptic.Commons.Threading.Tasks;

namespace iSynaptic.Modeling.Domain
{
    public abstract class AggregateRepository : IAggregateRepository, IAggregateRepositoryInternal
    {
        private static readonly Task _completedTask = Task.FromResult(true);

        public async Task<IAggregate> Get(Type aggregateType, object id, int maxVersion = int.MaxValue)
        {
            if (!aggregateType.DoesImplementType(typeof(IAggregate)))
                throw new ArgumentException("AggregateType must implement IAggregate.");

            return await ((IAggregateRepositoryInternal)this).GetCore<IAggregate>(aggregateType, id, maxVersion);
        }

        async Task<T> IAggregateRepositoryInternal.GetCore<T>(Type aggregateType, object id, int maxVersion)
        {
            var memento = await GetMemento(id, maxVersion);

            if (memento == null)
                return null;

            var aggregate = (IAggregate) FormatterServices.GetSafeUninitializedObject(memento.AggregateType);
            var ag = AsInternal(aggregate);
            ag.Initialize(memento);

            return (T)aggregate;
        }

        public async Task Save(IAggregate aggregate)
        {
            Guard.NotNull(aggregate, "aggregate");

            var ag = AsInternal(aggregate);
            var aggregateType = aggregate.GetType();
            var events = aggregate.GetUncommittedEvents().ToArray();

            if (events.Length <= 0)
                return;

            var firstEvent = events[0];

            int saveAttempts = 0;
            while (true)
            {
                var data = new AggregateEventsSaveFrame(aggregateType, aggregate.Id, firstEvent.Version == 1, firstEvent.Version - 1, aggregate.Version, events);

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
            await OnEventStreamSaved(aggregateType, aggregate.Id, events);
        }

        public async Task SaveSnapshot(IAggregate aggregate)
        {
            Guard.NotNull(aggregate, "aggregate");

            var snapshot = aggregate.TakeSnapshot();
            if (snapshot == null)
                throw new ArgumentException("Aggregate doesn't support snapshots.", "aggregate");

            var ag = AsInternal(aggregate);
            var aggregateType = aggregate.GetType();

            var events = aggregate.GetUncommittedEvents().ToArray();
            bool isNew = events.Length > 0 && events[0].Version == 1;

            var data = new AggregateSnapshotSaveFrame(aggregateType, aggregate.Id, isNew, snapshot);
            await SaveSnapshot(data);

            await OnSnapshotSaved(aggregateType, snapshot);
        }

        public async Task SaveEvents(Type aggregateType, object id, IEnumerable<IAggregateEvent> events)
        {
            Guard.NotNull(aggregateType, "aggregateType");
            Guard.NotNull(id, "id");
            Guard.NotNull(events, "events");

            var eventsToSave = events as IAggregateEvent[] ?? events.ToArray();
            if (eventsToSave.Length <= 0)
                return;

            var firstEvent = eventsToSave[0];
            var data = new AggregateEventsSaveFrame(aggregateType, id, firstEvent.Version == 1, firstEvent.Version - 1, firstEvent.Version + eventsToSave.Length, eventsToSave);

            await SaveEvents(data);
            await OnEventStreamSaved(aggregateType, id, eventsToSave);
        }

        private IAggregateInternal AsInternal(IAggregate aggregate)
        {
            var ag = aggregate as IAggregateInternal;
            if (ag == null) throw new InvalidOperationException("Aggregate must implement IAggregate.");
            return ag;
        }

        protected virtual Task OnEventStreamSaved(Type aggregateType, object id, IEnumerable<IAggregateEvent> events) { return _completedTask; }
        protected virtual Task OnSnapshotSaved(Type aggregateType, IAggregateSnapshot snapshot) { return _completedTask; }

        public async Task<AggregateMemento> GetMemento(object id, Int32 maxVersion)
        {
            var snapshotFrame = await GetSnapshot(id, maxVersion);
            if (snapshotFrame != null)
            {
                var snapshot = snapshotFrame.Snapshot;
                var events = Enumerable.Empty<IAggregateEvent>();

                if (snapshot.Version < maxVersion)
                {
                    var eventsFrame = await GetEvents(id, snapshot.Version + 1, maxVersion);
                    if (eventsFrame != null)
                        events = eventsFrame.Events;
                }

                return new AggregateMemento(snapshotFrame.AggregateType, snapshot.ToMaybe(), events);
            }
            else
            {
                var eventsFrame = await GetEvents(id, 1, maxVersion);

                return eventsFrame != null
                    ? new AggregateMemento(eventsFrame.AggregateType, Maybe.NoValue, eventsFrame.Events)
                    : null;
            }
        }

        protected abstract Task<AggregateSnapshotLoadFrame> GetSnapshot(object id, Int32 maxVersion);
        protected abstract Task<AggregateEventsLoadFrame> GetEvents(object id, Int32 minVersion, Int32 maxVersion);

        protected abstract Task SaveSnapshot(AggregateSnapshotSaveFrame frame);
        protected abstract Task SaveEvents(AggregateEventsSaveFrame frame);
    }

    internal interface IAggregateRepositoryInternal
    {
        Task<T> GetCore<T>(Type aggregateId, object id, Int32 maxVersion)
            where T : class, IAggregate;
    }

    public abstract class AggregateRepository<TAggregate, TIdentifier> : IAggregateRepository<TAggregate, TIdentifier> 
        where TAggregate : class, IAggregate<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly IAggregateRepository _innerRepository;
        private readonly IAggregateRepositoryInternal _internalRepository;

        protected AggregateRepository(IAggregateRepository innerRepository)
        {
            _innerRepository = Guard.NotNull(innerRepository, "innerRepository");
            _internalRepository = innerRepository as IAggregateRepositoryInternal;

            if (_internalRepository == null)
            {
                throw new ArgumentException("Inner IAggregateRepository must implement AggregateRepository.");
            }
        }

        ITask<TAggregate> IAggregateRepositoryQueries<TAggregate, TIdentifier>.Get(TIdentifier id, Int32 maxVersion)
        {
            return _internalRepository.GetCore<TAggregate>(typeof (TAggregate), id, maxVersion).ToCovariantTask();
        }

        Task IAggregateRepositoryCommands<TAggregate, TIdentifier>.Save(TAggregate aggregate)
        {
            return _innerRepository.Save(aggregate);
        }

        Task IAggregateRepositoryCommands<TAggregate, TIdentifier>.SaveSnapshot(TAggregate aggregate)
        {
            return _innerRepository.SaveSnapshot(aggregate);
        }
    }
}