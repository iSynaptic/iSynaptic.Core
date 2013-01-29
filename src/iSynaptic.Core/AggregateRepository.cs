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
using System.Runtime.Serialization;
using System.Threading.Tasks;
using iSynaptic.Commons;

namespace iSynaptic
{
    public abstract class AggregateRepository<TAggregate, TIdentifier>
        where TAggregate : Aggregate<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        public Task<TAggregate> Get(TIdentifier id)
        {
            return Get(id, Int32.MaxValue);
        }

        public async Task<TAggregate> Get(TIdentifier id, Int32 maxVersion)
        {
            var memento = await GetMemento(id, maxVersion);

            if (memento.IsEmpty)
                return null;

            var aggregateType = typeof(TAggregate);

            var aggregate = (TAggregate)FormatterServices.GetSafeUninitializedObject(aggregateType);
            aggregate.Initialize(memento);

            return aggregate;
        }

        public async Task Save(TAggregate aggregate)
        {
            Guard.NotNull(aggregate, "aggregate");

            if (SaveBehavior == AggregateRepositorySaveBehavior.SaveEventsOnly || SaveBehavior == AggregateRepositorySaveBehavior.SaveBothEventsAndSnapshot)
                await SaveEventStream(aggregate.Id, aggregate.GetUncommittedEvents());

            if (SaveBehavior == AggregateRepositorySaveBehavior.SaveSnapshotOnly || SaveBehavior == AggregateRepositorySaveBehavior.SaveBothEventsAndSnapshot)
                await SaveSnapshot(aggregate);

            aggregate.CommitEvents();
        }

        public Task SaveSnapshot(TIdentifier id)
        {
            return SaveSnapshot(id, Int32.MaxValue);
        }

        public async Task SaveSnapshot(TIdentifier id, Int32 maxVersion)
        {
            var aggregate = await Get(id, maxVersion);
            if (aggregate == null)
                throw new InvalidOperationException("Unable to find aggregate.");

            await SaveSnapshot(aggregate);
        }

        public async Task SaveSnapshot(TAggregate aggregate)
        {
            Guard.NotNull(aggregate, "aggregate");

            var snapshot = aggregate.TakeSnapshot();
            if (snapshot == null)
                throw new ArgumentException("Aggregate doesn't support snapshots.", "aggregate");

            await SaveSnapshot(snapshot);
        }

        protected virtual AggregateRepositorySaveBehavior SaveBehavior { get { return AggregateRepositorySaveBehavior.SaveEventsOnly; } }

        public abstract Task<AggregateMemento<TIdentifier>> GetMemento(TIdentifier id, Int32 maxVersion);

        protected abstract Task SaveSnapshot(AggregateSnapshot<TIdentifier> snapshot);
        protected abstract Task SaveEventStream(TIdentifier id, IEnumerable<AggregateEvent<TIdentifier>> events);
    }
}