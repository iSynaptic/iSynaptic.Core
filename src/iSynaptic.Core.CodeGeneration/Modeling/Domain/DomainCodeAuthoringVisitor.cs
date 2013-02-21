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

using System.IO;
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public class DomainCodeAuthoringVisitor : CSharpCodeAuthoringVisitor<Unit>
    {
        private readonly SymbolTable _symbolTable;

        public DomainCodeAuthoringVisitor(TextWriter writer, SymbolTable symbolTable) : base(writer)
        {
            _symbolTable = Guard.NotNull(symbolTable, "symbolTable");
        }

        protected void Visit(NamespaceSyntax @namespace)
        {
            using (WriteBlock("namespace {0}", @namespace.Name))
            {
                DispatchChildren(@namespace);
            }
        }

        protected void Visit(UsingStatementSyntax @using)
        {
            WriteLine("using {0};", @using.Namespace);
        }

        protected void Visit(ValueSyntax value)
        {
            using (WriteBlock("public class {0} : IEquatable<{0}>", value.Name))
            {
                
            }
        }

        protected void Visit(AggregateSyntax value)
        {
            using (WriteBlock("public class {0} : Aggregate<{1}>", value.Name, value.IdentifierType.ValueOrDefault(Syntax.IdentifierName("T"))))
            {

            }
        }
    }
}
