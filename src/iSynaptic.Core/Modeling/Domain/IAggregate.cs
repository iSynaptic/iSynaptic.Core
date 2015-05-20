using System;
using System.Collections.Generic;

namespace iSynaptic.Modeling.Domain
{
    public interface IAggregate
    {
        object Id { get; }
        int Version { get; }

        IEnumerable<IAggregateEvent> GetUncommittedEvents();
        IEnumerable<IAggregateEvent> GetEvents();
        
        IAggregateSnapshot TakeSnapshot();
    }

    public interface IAggregate<out TIdentifier> : IAggregate 
        where TIdentifier : IEquatable<TIdentifier>
    {
        new TIdentifier Id { get; }

        new IEnumerable<IAggregateEvent<TIdentifier>> GetUncommittedEvents();
    }
}