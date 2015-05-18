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

    public interface IAggregate<out TIdentifier> 
        where TIdentifier : IEquatable<TIdentifier>
    {
        TIdentifier Id { get; }
        Int32 Version { get; }

        IEnumerable<IAggregateEvent<TIdentifier>> GetUncommittedEvents();

        IAggregateSnapshot<TIdentifier> TakeSnapshot();
    }
}