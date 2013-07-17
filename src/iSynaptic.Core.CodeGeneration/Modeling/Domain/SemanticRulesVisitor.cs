using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons;
using iSynaptic.Modeling.Domain;
using ISymbol = iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel.ISymbol;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public partial class SemanticRulesVisitor : ValidationVisitor<SemanticRuleViolation>
    {
        private SemanticRulesVisitor(SymbolTable symbolTable) : base(symbolTable)
        {
        }

        public static Outcome<SemanticRuleViolation> Validate(Compilation compilation, SymbolTable symbolTable)
        {
            Guard.NotNull(compilation, "compilation");
            Guard.NotNull(symbolTable, "symbolTable");

            var visitor = new SemanticRulesVisitor(symbolTable);
            visitor.Validate(compilation);

            return visitor.Result;
        }

        private Maybe<ISymbol> ResolveAndValidateTypeExists(ISymbol context, NameSyntax name)
        {
            var resolution = SymbolTable.Resolve(context, name);
            if (resolution.Status == SymbolResolutionStatus.Found)
                return resolution.Symbol.ToMaybe();

            if (resolution.Status == SymbolResolutionStatus.Ambiguous)
            {
                Fail(SemanticRuleViolation.TypeReferenceIsAmbiguous(context, name, resolution));
                return Maybe.NoValue;
            }

            Fail(SemanticRuleViolation.TypeOrNamespaceDoesNotExist(context, name));
            return Maybe.NoValue;
        }
    }
}
