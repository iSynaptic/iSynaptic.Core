using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using iSynaptic.TestDomain;
using NUnit.Framework;

namespace iSynaptic.Modeling.Domain
{
    [TestFixture]
    public class ValueObjectTests
    {
        [Test]
        public void EssenceBehavesCorrectly()
        {
            var s = new CommunicationThreadSnapshot(42, "AT", "AD");
            
            CommunicationThreadSnapshot.Essence e = s.ToEssence();
            e.ThreadId = 84;

            s = e.Create();

            s.ThreadId.Should().Be(84);
            s.Topic.Should().Be("AT");
            s.Description.Should().Be("AD");
        }
    }
}
