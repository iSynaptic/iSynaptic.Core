using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public class DomainCodeAuthoringSettings
    {
        public DomainCodeAuthoringSettings()
        {
            EventGenerationSite = AggregateEventGenerationSite.NestedInAggregate;
        }

        public AggregateEventGenerationSite EventGenerationSite { get; set; }
    }
}
