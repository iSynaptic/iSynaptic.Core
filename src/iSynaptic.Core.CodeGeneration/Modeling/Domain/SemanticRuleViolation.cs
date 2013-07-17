using System;
using System.Linq;
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons.Linq;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public class SemanticRuleViolation
    {
        public static class RuleCodes
        {
            public static readonly int TypeOrNamespaceDoesNotExist = 1000;
            public static readonly int TypeReferenceIsAmbiguous = 1001;

            #region Aggregate Rule Codes
            
            public static readonly int AggregateMustHaveIdentifier = 1100;
            public static readonly int AggregateIdentifierMustBeAValueObject = 1101;

            #endregion
        }

        public static SemanticRuleViolation TypeOrNamespaceDoesNotExist(INode subject, NameSyntax name)
        {
            return new SemanticRuleViolation(
                RuleCodes.TypeOrNamespaceDoesNotExist,
                String.Format("The type or namespace '{0}' could not be found.", name),
                subject);
        }

        public static SemanticRuleViolation TypeReferenceIsAmbiguous(INode subject, NameSyntax name, SymbolResolution resolution)
        {
            var symbols = resolution.Symbols.ToArray();
            string ambiguousTypes = 
                symbols.Take(symbols.Length - 1).Delimit(", ", x => String.Format("'{0}'", x.FullName))
                + String.Format(" and '{0}'", symbols[symbols.Length - 1].FullName);

            return new SemanticRuleViolation(
                RuleCodes.TypeReferenceIsAmbiguous,
                String.Format("'{0}' is an ambiguous reference between {1}.", name, ambiguousTypes),
                subject);
        }

        #region Aggregate Rules
        
        public static SemanticRuleViolation AggregateMustHaveIdentifier(AggregateSyntax aggregate)
        {
            return new SemanticRuleViolation(
                RuleCodes.AggregateMustHaveIdentifier,
                String.Format("Aggregate '{0}' must have an identifier, either by inheriting from a base aggregate or by specifying an identifier type.", aggregate.Name),
                aggregate);
        }

        public static SemanticRuleViolation AggregateIdentifierMustBeAValueObject(AggregateSyntax aggregate)
        {
            return new SemanticRuleViolation(
                RuleCodes.AggregateIdentifierMustBeAValueObject,
                String.Format("Identifier for '{0}' must be a value object.", aggregate.Name),
                aggregate);
        }

        #endregion

        private SemanticRuleViolation(int ruleCode, string message, INode subject)
        {
            RuleCode = ruleCode;
            Message = message;
            Subject = subject;
        }

        public int RuleCode { get; private set; }
        public string Message { get; private set; }
        public INode Subject { get; private set; }
    }
}