using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    [Flags]
    public enum DomainTypes : byte
    {
        All = 0xFF,

        Values = 1 << 0,
        Aggregates = 1 << 1,
        AggregateEvents = 1 << 2,
        AggregateSnapshots = 1 << 3
    }
}
