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
    public class InMemoryRepository<TAggregate, TIdentifier> : AggregateRepository<TAggregate, TIdentifier>
        where TAggregate : Aggregate<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly Dictionary<TIdentifier, AggregateEventStream<TIdentifier>> _events
            = new Dictionary<TIdentifier, AggregateEventStream<TIdentifier>>();

        private readonly Dictionary<TIdentifier, SortedSet<AggregateSnapshot<TIdentifier>>> _snapshots
            = new Dictionary<TIdentifier, SortedSet<AggregateSnapshot<TIdentifier>>>();

        private readonly Task _completedTask;

        public InMemoryRepository()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            _completedTask = tcs.Task;
        }

        public override Task<AggregateMemento<TIdentifier>> GetMemento(TIdentifier id, int maxVersion)
        {
            lock (_events)
            {
                var snapshot = GetRecentSnapshot(id, maxVersion);

                var events = _events
                    .TryGetValue(id)
                    .Select(x => x.CommittedEvents)
                    .Squash()
                    .Where(x => x.Version <= maxVersion &&
                                x.Version > snapshot.Select(y => y.Version).ValueOrDefault());

                return Task.FromResult(new AggregateMemento<TIdentifier>(snapshot, events));
            }
        }

        private Maybe<AggregateSnapshot<TIdentifier>> GetRecentSnapshot(TIdentifier id, int maxVersion)
        {
            return _snapshots
                .TryGetValue(id)
                .OnNoValue(() => _snapshots.Add(id, new SortedSet<AggregateSnapshot<TIdentifier>>()))
                .SelectMaybe(x => x.TakeWhile(y => y.Version <= maxVersion).TryLast())
                .Where(x => x.Version <= maxVersion);
        }

        protected override Task SaveSnapshot(AggregateSnapshot<TIdentifier> snapshot)
        {
            lock (_events)
            {
                var mostRecentSnapshot = GetRecentSnapshot(snapshot.Id, snapshot.Version);
                if (mostRecentSnapshot.Select(x => x.Version).ValueOrDefault() == snapshot.Version)
                    throw new InvalidOperationException("Snapshot for this version already exists.");

                _snapshots[snapshot.Id].Add(snapshot);
            }

            return _completedTask;
        }

        protected override Task SaveEventStream(TIdentifier id, IEnumerable<AggregateEvent<TIdentifier>> events)
        {
            lock (_events)
            {
                var eventStream = _events
                    .TryGetValue(id)
                    .OnNoValue(() => _events.Add(id, new AggregateEventStream<TIdentifier>()))
                    .Or(() => _events.TryGetValue(id))
                    .ValueOrDefault();

                events.Run(eventStream.AppendEvent);
                eventStream.CommitEvents();
            }

            return _completedTask;
        }
    }
}