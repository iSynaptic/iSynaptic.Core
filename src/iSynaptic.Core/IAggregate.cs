using System;

namespace iSynaptic
{
    public interface IAggregate<out TIdentifier> 
        where TIdentifier : IEquatable<TIdentifier>
    {
        TIdentifier Id { get; }
        Int32 Version { get; }

        IAggregateSnapshot<TIdentifier> TakeSnapshot();
    }
}