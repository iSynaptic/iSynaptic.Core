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
    public class FullDomainCodeAuthoringVisitor : DomainCodeAuthoringVisitor<Unit>
    {
        public FullDomainCodeAuthoringVisitor(TextWriter writer, SymbolTable symbolTable) 
            : base(writer, symbolTable)
        {
        }

        protected void Visit(SyntaxTree tree)
        {
            Dispatch(tree.UsingStatements.Concat(new[]
            {
                Syntax.UsingStatement((NameSyntax)"System"),
                Syntax.UsingStatement((NameSyntax)"System.Collections.Generic"),
                Syntax.UsingStatement((NameSyntax)"System.Linq"),
                Syntax.UsingStatement((NameSyntax)"iSynaptic.Commons")

            }).Distinct(x => x.Namespace));
            WriteLine();

            Dispatch(tree.Namespaces);
        }

        protected void Visit(NamespaceSyntax @namespace)
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

        protected void Visit(UsingStatementSyntax @using)
        {
            WriteLine("using {0};", @using.Namespace);
        }

        protected void Visit(ValueSyntax value)
        {
            new ValueCodeAuthoringVisitor(Writer, SymbolTable).Dispatch(value);
        }

        protected void Visit(AggregateSyntax aggregate)
        {
            var baseAggregateSymbol = aggregate.Base
                .Select(x => SymbolTable.Resolve(aggregate, x))
                .Select(x => x.Symbol);

            if (baseAggregateSymbol.HasValue && (baseAggregateSymbol.Value == aggregate || !(baseAggregateSymbol.Value is AggregateSyntax)))
                throw new InvalidOperationException("Aggregate cannot be it's own base or use non-aggregates as a base.");

            String baseAggregate = baseAggregateSymbol
                .Select(x => x.FullName)
                .Select(x => GetRelativeName(x, aggregate.Parent))
                .Select(x => x.ToString())
                .ValueOrDefault("Aggregate<Guid>");

            using (WriteBlock("public class {0} : {1}", aggregate.Name, baseAggregate))
            {

            }
        }
    }
}
