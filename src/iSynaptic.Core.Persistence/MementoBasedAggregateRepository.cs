using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iSynaptic.Commons;
using iSynaptic.Commons.Collections.Generic;
using iSynaptic.Commons.Linq;
using iSynaptic.Modeling;
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

        public override Task<AggregateMemento<TIdentifier>> GetMemento(TIdentifier id, int maxVersion)
        {
            return Task.FromResult(TryLoadMemento(id)
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

        protected override Task SaveSnapshot(Type aggregateType, IAggregateSnapshot<TIdentifier> snapshot)
        {
            StoreMemento(() =>
            {
                var state = TryLoadMemento(snapshot.Id).ValueOrDefault();

                return KeyValuePair.Create(snapshot.Id, new AggregateMemento<TIdentifier>(aggregateType, snapshot.ToMaybe(), state != null ? state.Events : null));
            });

            return _completedTask;
        }

        protected override Task SaveEventStream(Type aggregateType, TIdentifier id, IEnumerable<IAggregateEvent<TIdentifier>> events)
        {
            StoreMemento(() => 
                KeyValuePair.Create(id, TryLoadMemento(id)
                    .Select(x =>
                    {
                        var lastEvent = x.Events.TryLast();
                        return new AggregateMemento<TIdentifier>(
                            aggregateType,
                            x.Snapshot,
                            events.SkipWhile(y => y.Version <= lastEvent.Select(z => z.Version).ValueOrDefault()));
                    })
                    .ValueOrDefault(() => new AggregateMemento<TIdentifier>(aggregateType, Maybe<IAggregateSnapshot<TIdentifier>>.NoValue, events))));

            return _completedTask;
        }
    }
}