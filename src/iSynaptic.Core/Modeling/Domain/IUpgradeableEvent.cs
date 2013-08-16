using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSynaptic.Modeling.Domain
{
    public interface IUpgradeableEvent<out TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        IAggregateEvent<TIdentifier> UpgradeEvent();
    }
}
