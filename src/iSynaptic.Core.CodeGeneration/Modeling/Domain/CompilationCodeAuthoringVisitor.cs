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

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public class CompilationCodeAuthoringVisitor : DomainCodeAuthoringVisitor<Unit>
    {
        public CompilationCodeAuthoringVisitor(TextWriter writer, SymbolTable symbolTable) 
            : base(writer, symbolTable)
        {
        }

        protected virtual void Visit(Compilation compilation)
        {
            var topLevelUsings = new[]
            {
                Syntax.UsingStatement((NameSyntax) "System"),
                Syntax.UsingStatement((NameSyntax) "System.Collections.Generic"),
                Syntax.UsingStatement((NameSyntax) "System.Linq"),
                Syntax.UsingStatement((NameSyntax) "iSynaptic"),
                Syntax.UsingStatement((NameSyntax) "iSynaptic.Commons"),
                Syntax.UsingStatement((NameSyntax) "iSynaptic.Modeling.Domain")

            }
            .Concat(compilation.Trees.SelectMany(x => x.UsingStatements))
            .Distinct(x => x.Namespace);

            Dispatch(topLevelUsings);
            WriteLine();

            DispatchChildren(compilation);
        }

        protected virtual void Visit(SyntaxTree tree)
        {
            if (tree.Parent == null && tree.UsingStatements.Any())
            {
                Dispatch(tree.UsingStatements);
                WriteLine();
            }

            Dispatch(tree.Namespaces);
        }

        protected virtual void Visit(NamespaceSyntax @namespace)
        {
            using (WriteBlock("namespace {0}", @namespace.Name))
            {
                if (@namespace.UsingStatements.Any())
                {
                    Dispatch(@namespace.UsingStatements);
                    WriteLine();
                }

                Delimit(@namespace.Members, new Unit(), Environment.NewLine);
            }
        }

        protected virtual void Visit(EnumSyntax @enum)
        {
            if (@enum.IsExternal)
                return;

            using (WriteBlock("public enum {0}", @enum.Name))
            {
                WriteLine(@enum.Values.Select(x => x.SimpleName).Delimit(",\r\n"));
            }
        }

        protected virtual void Visit(UsingStatementSyntax @using)
        {
            WriteLine("using {0};", @using.Namespace);
        }

        protected virtual void Visit(ValueSyntax value)
        {
            new ValueCodeAuthoringVisitor(Writer, SymbolTable).Dispatch(value);
        }

        protected virtual void Visit(AggregateSyntax aggregate)
        {
            new AggregateCodeAuthoringVisitor(Writer, SymbolTable).Dispatch(aggregate);
        }

        protected virtual void Visit(ScalarValueSyntax value)
        {
            new ScalarValueCodeAuthoringVisitor(Writer, SymbolTable).Dispatch(value);
        }
    }
}
