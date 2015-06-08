using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel
{
    public enum Visibility
    {
        Public,
        Protected,
        Internal,
        Private
    }

    public static class VisibilityExtensions
    {
        public static string ToCSharpModifier(this Visibility visibility)
        {
            if (visibility == Visibility.Public) return "public";
            if (visibility == Visibility.Protected) return "protected";
            if (visibility == Visibility.Internal) return "internal";
            if (visibility == Visibility.Private) return "private";

            throw new InvalidOperationException("Unrecognized visibility.");
        }
    }
}
