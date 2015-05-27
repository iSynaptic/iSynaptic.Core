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
            AggregateComponentSite = ComponentTypeSite.Nested;

            TypesToGenerate = DomainTypes.All;
        }

        public ComponentTypeSite AggregateComponentSite { get; set; }

        public DomainTypes TypesToGenerate { get; set; }
    }
}
