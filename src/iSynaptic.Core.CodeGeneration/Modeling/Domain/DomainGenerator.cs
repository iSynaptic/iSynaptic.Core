using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public static class DomainGenerator
    {
        public static void Generate(DomainCodeAuthoringSettings settings, Compilation compilation, SymbolTable symbolTable, TextWriter target)
        {
            Guard.NotNull(settings, nameof(settings));
            Guard.NotNull(compilation, nameof(compilation));
            Guard.NotNull(symbolTable, nameof(symbolTable));
            Guard.NotNull(target, nameof(target));

            var visitor = new CompilationCodeAuthoringVisitor(target, symbolTable, settings);
            visitor.Dispatch(compilation);
        }
    }
}
