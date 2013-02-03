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

namespace iSynaptic
{
    public class InMemoryAggregateRepository<TAggregate, TIdentifier> : AggregateRepository<TAggregate, TIdentifier>
        where TAggregate : class, IAggregate<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly Dictionary<TIdentifier, AggregateMemento<TIdentifier>> _state =
            new Dictionary<TIdentifier, AggregateMemento<TIdentifier>>();

        private readonly Task _completedTask;

        public InMemoryAggregateRepository()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            _completedTask = tcs.Task;
        }

        public override Task<AggregateMemento<TIdentifier>> GetMemento(TIdentifier id, int maxVersion)
        {
            lock (_state)
            {
                return Task.FromResult(_state.TryGetValue(id)
                      .Select(x =>
                      {
                          var s = x.Snapshot.Where(y => y.Version <= maxVersion);

                          return new AggregateMemento<TIdentifier>(
                              x.AggregateType,
                              s,
                              x.Events.SkipWhile(y => y.Version <= s.Select(z => z.Version).ValueOrDefault())
                               .TakeWhile(y => y.Version <= maxVersion));
                      })
                      .ValueOrDefault());
            }
        }

        protected override Task SaveSnapshot(Type aggregateType, IAggregateSnapshot<TIdentifier> snapshot)
        {
            lock (_state)
            {
                var state = _state.TryGetValue(snapshot.Id).ValueOrDefault();

                if (state != null && state.Snapshot.Where(x => x.Version >= snapshot.Version).HasValue)
                    throw new InvalidOperationException("Snapshot already exists that covers this aggregate version.");

                _state[snapshot.Id] = new AggregateMemento<TIdentifier>(aggregateType, snapshot.ToMaybe(), state != null ? state.Events : null);
            }

            return _completedTask;
        }

        protected override Task SaveEventStream(Type aggregateType, TIdentifier id, IEnumerable<IAggregateEvent<TIdentifier>> events)
        {
            lock (_state)
            {
                _state[id] = _state.TryGetValue(id)
                    .Select(x =>
                    {
                        var lastEvent = x.Events.TryLast();
                        return new AggregateMemento<TIdentifier>(
                            aggregateType,
                            x.Snapshot,
                            events.SkipWhile(y => y.Version <= lastEvent.Select(z => z.Version).ValueOrDefault()));
                    })
                    .ValueOrDefault(() => new AggregateMemento<TIdentifier>(aggregateType, Maybe<IAggregateSnapshot<TIdentifier>>.NoValue, events));
            }

            return _completedTask;
        }
    }
}