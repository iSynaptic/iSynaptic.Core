using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Sprache;
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    [TestFixture]
    public class SemanticRulesVisitorTests
    {
        private Outcome<SemanticRuleViolation> Validate(string input)
        {
            var compilation = Syntax.Compilation(
                new[]{Parser.SyntaxTree
                .Parse(input)}
            );

            var symbolTable = SymbolTableConstructionVisitor.BuildSymbolTable(compilation);

            return SemanticRulesVisitor.Validate(compilation, symbolTable);
        }

        [Test]
        public void Aggregate_MustHaveIdentifier()
        {
            var outcome = Validate("namespace Test { aggregate Foo { } }");
            outcome.WasSuccessful.Should().BeFalse();
            outcome.Observations.Count().Should().Be(1);

            var violation = outcome.Observations.Single(x => x.RuleCode == SemanticRuleViolation.RuleCodes.AggregateMustHaveIdentifier);

            violation.Should().NotBeNull();
            violation.Subject.Should().BeOfType<AggregateSyntax>();
        }

        [Test]
        public void Aggregate_IdentifierTypeMustExist()
        {
            var outcome = Validate("namespace Test { aggregate<BazId> Foo { } }");
            outcome.WasSuccessful.Should().BeFalse();
            outcome.Observations.Count().Should().Be(1);

            var violation = outcome.Observations.Single(x => x.RuleCode == SemanticRuleViolation.RuleCodes.TypeOrNamespaceDoesNotExist);

            violation.Should().NotBeNull();
            violation.Subject.Should().BeOfType<AggregateSyntax>();
        }

        [Test]
        public void Aggregate_IdentifierMustBeAValueObject()
        {
            var outcome = Validate("namespace Test { aggregate<int> Baz { } aggregate<Baz> Foo { } }");
            outcome.WasSuccessful.Should().BeFalse();
            outcome.Observations.Count().Should().Be(1);
            var violation = outcome.Observations.Single(x => x.RuleCode == SemanticRuleViolation.RuleCodes.AggregateIdentifierMustBeAValueObject);

            violation.Should().NotBeNull();
            violation.Subject.Should().BeOfType<AggregateSyntax>();
        }
    }
}
