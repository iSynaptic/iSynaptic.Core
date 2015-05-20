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
    public abstract class MementoBasedAggregateRepository : AggregateRepository
    {
        protected abstract Task<Maybe<AggregateMemento>> TryLoadMemento(object id);
        protected abstract Task StoreMemento(Func<Task<KeyValuePair<object, AggregateMemento>>> mementoFactory);

        protected override async Task<AggregateSnapshotLoadFrame> GetSnapshot(object id, int maxVersion)
        {
            return (await TryLoadMemento(id))
                .Where(x => x.Snapshot.Select(y => y.Version <= maxVersion).ValueOrDefault())
                .Select(x => new AggregateSnapshotLoadFrame(x.AggregateType, id, x.Snapshot.Value))
                .ValueOrDefault();
        }

        protected override async Task<AggregateEventsLoadFrame> GetEvents(object id, int minVersion, int maxVersion)
        {
            return (await TryLoadMemento(id))
                .Select(x => new AggregateEventsLoadFrame(
                    x.AggregateType,
                    id,
                    x.Events
                        .SkipWhile(y => y.Version < minVersion)
                        .TakeWhile(y => y.Version <= maxVersion)))
                .ValueOrDefault();
        }

        protected override Task SaveSnapshot(AggregateSnapshotSaveFrame frame)
        {
            return StoreMemento(async () =>
            {
                var aggregateType = frame.AggregateType;
                var snapshot = frame.Snapshot;

                var state = (await TryLoadMemento(snapshot.Id)).ValueOrDefault();

                return KeyValuePair.Create(snapshot.Id, new AggregateMemento(aggregateType, snapshot.ToMaybe(), state != null ? state.Events : null));
            });
        }

        protected override Task SaveEvents(AggregateEventsSaveFrame frame)
        {
            var aggregateType = frame.AggregateType;
            var id = frame.Id;
            var events = frame.Events.ToArray();

            return StoreMemento(async () =>
                KeyValuePair.Create(id, (await TryLoadMemento(id))
                    .Select(x =>
                    {
                        var lastEvent = x.Events.TryLast();
                        var actualVersion = lastEvent.Select(y => y.Version).ValueOrDefault();

                        if (actualVersion != frame.ExpectedVersion)
                            throw new AggregateConcurrencyException();

                        return new AggregateMemento(
                            aggregateType,
                            x.Snapshot,
                            x.Events.Concat(events.SkipWhile(y => y.Version <= lastEvent.Select(z => z.Version).ValueOrDefault())));
                    })
                    .ValueOrDefault(() => new AggregateMemento(aggregateType, Maybe<IAggregateSnapshot>.NoValue, events))));
        }
    }
}