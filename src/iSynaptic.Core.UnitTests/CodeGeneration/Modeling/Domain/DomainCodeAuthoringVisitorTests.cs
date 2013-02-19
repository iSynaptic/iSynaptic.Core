using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    [TestFixture]
    public class DomainCodeAuthoringVisitorTests
    {
        [Test]
        public void CanGenerateCode()
        {
            var visitor = new DomainCodeAuthoringVisitor(Console.Out);

            var tree = Syntax.SyntaxTree(Enumerable.Empty<UsingStatementSyntax>(),
                                         new[]{Syntax.Namespace("Test",
                                                                Enumerable.Empty<UsingStatementSyntax>(),
                                                                Enumerable.Empty<NamespaceSyntax>(),
                                                                Enumerable.Empty<AggregateSyntax>(),
                                                                Enumerable.Empty<ValueSyntax>())});

            visitor.Dispatch(tree);
        }
    }
}
