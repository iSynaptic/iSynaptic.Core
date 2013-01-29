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
    public abstract class AggregateRepository<TAggregate, TIdentifier> : IAggregateRepository<TAggregate, TIdentifier> 
        where TAggregate : class, IAggregate<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        public async Task<TAggregate> Get(TIdentifier id, Int32 maxVersion)
        {
            var memento = await GetMemento(id, maxVersion);

            if (memento.IsEmpty())
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

            await SaveEventStream(aggregate.GetType(), aggregate.Id, ag.GetUncommittedEvents());
            ag.CommitEvents();
        }

        public async Task SaveSnapshot(TAggregate aggregate)
        {
            Guard.NotNull(aggregate, "aggregate");

            var snapshot = aggregate.TakeSnapshot();
            if (snapshot == null)
                throw new ArgumentException("Aggregate doesn't support snapshots.", "aggregate");

            await SaveSnapshot(aggregate.GetType(), snapshot);
        }

        private IAggregateInternal<TIdentifier> AsInternal(TAggregate aggregate)
        {
            var ag = aggregate as IAggregateInternal<TIdentifier>;

            if(ag == null) throw new InvalidOperationException("Aggregate must inherit from Aggregate<TIdentifier>.");
            return ag;
        }

        public abstract Task<AggregateMemento<TIdentifier>> GetMemento(TIdentifier id, Int32 maxVersion);

        protected abstract Task SaveSnapshot(Type aggregateType, IAggregateSnapshot<TIdentifier> snapshot);
        protected abstract Task SaveEventStream(Type aggregateType, TIdentifier id, IEnumerable<IAggregateEvent<TIdentifier>> events);
    }
}