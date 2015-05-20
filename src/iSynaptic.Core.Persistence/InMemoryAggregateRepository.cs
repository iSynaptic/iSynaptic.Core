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
using System.Threading;
using System.Threading.Tasks;
using iSynaptic.Commons;
using iSynaptic.Commons.Collections.Generic;
using iSynaptic.Modeling.Domain;

namespace iSynaptic.Core.Persistence
{
    public class InMemoryAggregateRepository : MementoBasedAggregateRepository
    {
        private readonly Dictionary<object, AggregateMemento> _state = new Dictionary<object, AggregateMemento>();

        protected override Task<Maybe<AggregateMemento>> TryLoadMemento(object id)
        {
            lock (_state)
            {
                return Task.FromResult(_state.TryGetValue(id));
            }
        }

        protected override async Task StoreMemento(Func<Task<KeyValuePair<object, AggregateMemento>>> mementoFactory)
        {
            bool lockTaken = false;

            try
            {
                Monitor.Enter(_state, ref lockTaken);

                var memento = await mementoFactory();
                _state[memento.Key] = memento.Value;

            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(_state);
            }
        }
    }

    public class InMemoryAggregateRepository<TAggregate, TIdentifier> : AggregateRepository<TAggregate, TIdentifier>
        where TAggregate : class, IAggregate<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        public InMemoryAggregateRepository()
            : base(new InMemoryAggregateRepository())
        {
        }
    }
}