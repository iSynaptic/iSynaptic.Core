// The MIT License
// 
// Copyright (c) 2013 Jordan E. Terrell
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Linq;
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;
using iSynaptic.Commons.Text;
using ISymbol = iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel.ISymbol;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public abstract class DomainCodeAuthoringVisitor<TState> : CSharpCodeAuthoringVisitor<TState>
    {
        private readonly SymbolTable _symbolTable;

        protected DomainCodeAuthoringVisitor(TextWriter writer, SymbolTable symbolTable) : base(writer)
        {
            _symbolTable = Guard.NotNull(symbolTable, "symbolTable");
        }

        protected DomainCodeAuthoringVisitor(IndentingTextWriter writer, SymbolTable symbolTable)
            : base(writer)
        {
            _symbolTable = Guard.NotNull(symbolTable, "symbolTable");
        }

        protected String GetTypeString(NameSyntax type, ISymbol relativeTo)
        {
            return GetTypeString(new TypeReferenceSyntax(type, new TypeCardinalitySyntax(1, 1)), relativeTo);
        }

        protected String GetTypeString(TypeReferenceSyntax reference, ISymbol relativeTo)
        {
            if (reference.Cardinality.IsMany)
                return String.Format("IEnumerable<{0}>", reference.Name);

            if (reference.Cardinality.IsOptional)
                return String.Format("Maybe<{0}>", reference.Name);

            return reference.Name;
        }

        protected NameSyntax GetRelativeName(NameSyntax name, ISymbol relativeTo)
        {
            Guard.NotNull(name, "name");
            Guard.NotNull(relativeTo, "relativeTo");

            if (name == relativeTo.FullName)
                return name.SimpleName;

            NameSyntax result = name;

            result = name.Parts
                         .ZipAll(relativeTo.FullName.Parts)
                         .SkipWhile(x => x[0] == x[1])
                         .SelectMaybe(x => x[0])
                         .OfType<NameSyntax>()
                         .Aggregate((l, r) => l + r);


            var usings = relativeTo as IUsingsContainer;
            if (usings != null)
            {
                var applicableUsing = usings
                    .UsingStatements
                    .Select(x => x.Namespace)
                    .Where(result.StartsWith)
                    .OrderByDescending(x => x.Parts.Count())
                    .TryFirst();

                if (applicableUsing.HasValue)
                    return result.Parts
                                 .Skip(applicableUsing.Value.Parts.Count())
                                 .Aggregate<NameSyntax>((l, r) => l + r);
            }

            return result;
        }

        protected SymbolTable SymbolTable { get { return _symbolTable; } }
    }
}