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
using iSynaptic.Commons.Collections.Generic;
using iSynaptic.Commons.Linq;
using iSynaptic.Modeling.Domain;

namespace iSynaptic.Core.Persistence
{
    public abstract class MementoBasedAggregateRepository<TAggregate, TIdentifier> : AggregateRepository<TAggregate, TIdentifier>
        where TAggregate : class, IAggregate<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly Task _completedTask;

        protected MementoBasedAggregateRepository()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            _completedTask = tcs.Task;
        }

        protected abstract Maybe<AggregateMemento<TIdentifier>> TryLoadMemento(TIdentifier id);
        protected abstract void StoreMemento(Func<KeyValuePair<TIdentifier, AggregateMemento<TIdentifier>>> mementoFactory);

        protected override Task<AggregateSnapshotFrame<TIdentifier>> GetSnapshot(TIdentifier id, int maxVersion)
        {
            var result = TryLoadMemento(id)
                .Where(x => x.Snapshot.Select(y => y.Version <= maxVersion).ValueOrDefault())
                .Select(x => new AggregateSnapshotFrame<TIdentifier>(x.AggregateType, id, x.Snapshot.Value))
                .ValueOrDefault();

            return Task.FromResult(result);
        }

        protected override Task<AggregateEventsFrame<TIdentifier>> GetEvents(TIdentifier id, int minVersion, int maxVersion)
        {
            var result = TryLoadMemento(id)
                .Select(x => new AggregateEventsFrame<TIdentifier>(
                                 x.AggregateType,
                                 id,
                                 x.Events
                                  .SkipWhile(y => y.Version < minVersion)
                                  .TakeWhile(y => y.Version <= maxVersion)))
                .ValueOrDefault();

            return Task.FromResult(result);
        }

        protected override Task SaveSnapshot(AggregateSnapshotFrame<TIdentifier> frame)
        {
            StoreMemento(() =>
            {
                var aggregateType = frame.AggregateType;
                var snapshot = frame.Snapshot;

                var state = TryLoadMemento(snapshot.Id).ValueOrDefault();

                return KeyValuePair.Create(snapshot.Id, new AggregateMemento<TIdentifier>(aggregateType, snapshot.ToMaybe(), state != null ? state.Events : null));
            });

            return _completedTask;
        }

        protected override Task SaveEvents(AggregateEventsFrame<TIdentifier> frame)
        {
            var aggregateType = frame.AggregateType;
            var id = frame.Id;
            var events = frame.Events.ToArray();

            StoreMemento(() => 
                KeyValuePair.Create(id, TryLoadMemento(id)
                    .Select(x =>
                    {
                        var lastEvent = x.Events.TryLast();
                        var expectedVersion = events[0].Version - 1;
                        var actualVersion = lastEvent.Select(y => y.Version).ValueOrDefault();

                        if(actualVersion != expectedVersion)
                            throw new AggregateConcurrencyException();

                        return new AggregateMemento<TIdentifier>(
                            aggregateType,
                            x.Snapshot,
                            x.Events.Concat(events.SkipWhile(y => y.Version <= lastEvent.Select(z => z.Version).ValueOrDefault())));
                    })
                    .ValueOrDefault(() => new AggregateMemento<TIdentifier>(aggregateType, Maybe<IAggregateSnapshot<TIdentifier>>.NoValue, events))));

            return _completedTask;
        }
    }
}