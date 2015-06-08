using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons.Linq;
using iSynaptic.Commons.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public class EnumCodeAuthoringVisitor : DomainCodeAuthoringVisitor<string>
    {
        public EnumCodeAuthoringVisitor(IndentingTextWriter writer, SymbolTable symbolTable, DomainCodeAuthoringSettings settings)
            : base(writer, symbolTable, settings)
        {
        }

        protected virtual void Visit(EnumSyntax @enum)
        {
            if (@enum.IsExternal)
                return;

            using (WriteBlock($"{@enum.Visibility.ToCSharpModifier()} enum {@enum.Name}"))
            {
                WriteLine(@enum.Values.Select(x => x.SimpleName).Delimit(",\r\n"));
            }
        }
    }
}
