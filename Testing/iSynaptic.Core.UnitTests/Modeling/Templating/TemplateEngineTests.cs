using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using iSynaptic.Core.TestingCommons;

namespace iSynaptic.Core.Modeling.Templating
{
    [TestFixture]
    public class TemplateEngineTests : BaseTestFixture
    {
        public class TestModel
        {
            public string Name { get; set; }
        }
    }
}
