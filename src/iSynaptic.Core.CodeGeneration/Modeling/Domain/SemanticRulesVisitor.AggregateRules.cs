using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons;
using iSynaptic.Modeling.Domain;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public partial class SemanticRulesVisitor
    {
        private void Visit(AggregateSyntax aggregate)
        {
            if (aggregate.Identifier.HasValue)
            {
                var id = aggregate.Identifier.Value;
                if (id is NamedAggregateIdentifierSyntax)
                    ValidateNamedAggregateIdentifier((NamedAggregateIdentifierSyntax) id, aggregate);

                if (id is GenericAggregateIdentifierSyntax)
                    ValidateGenericAggregateIdentifier((GenericAggregateIdentifierSyntax) id, aggregate);
            }
            else if(!aggregate.Base.HasValue)
                Fail(SemanticRuleViolation.AggregateMustHaveIdentifier(aggregate));
        }

        private void ValidateNamedAggregateIdentifier(NamedAggregateIdentifierSyntax id, AggregateSyntax aggregate)
        {
            var idSymbol = ResolveAndValidateTypeExists(aggregate, id.Type.Name);
            if (!idSymbol.HasValue) return;

            var idType = idSymbol.Value as IType;
            if (idType == null)
            {
                Fail(SemanticRuleViolation.AggregateIdentifierMustBeAValueObject(aggregate));
                return;
            }

            if (!idType.HasValueSemantics)
            {
                Fail(SemanticRuleViolation.AggregateIdentifierMustBeAValueObject(aggregate));
                return;
            }
        }

        private void ValidateGenericAggregateIdentifier(GenericAggregateIdentifierSyntax id, AggregateSyntax aggregate)
        {
            throw new System.NotImplementedException();
        }
    }
}
