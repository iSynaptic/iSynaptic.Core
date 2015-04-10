using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;

namespace iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel
{
    public static class SyntacticModelExtensions
    {
        public static IEnumerable<IType> GetInheritanceHierarchy(this IType type, SymbolTable symbolTable)
        {
            Func<IType, Maybe<IType>> getBaseType = t =>
            {
                var twb = t as ITypeWithBase;
                if (twb == null || !twb.Base.HasValue)  return default(Maybe<IType>);

                var parent = twb.Parent as ISymbol;
                if (parent == null) return default(Maybe<IType>);

                var res = symbolTable.Resolve(parent, twb.Base.Value);
                return (res.Symbol as IType).ToMaybe();
            };

            return type.Recurse(getBaseType).Skip(1);
        }
    }
}
