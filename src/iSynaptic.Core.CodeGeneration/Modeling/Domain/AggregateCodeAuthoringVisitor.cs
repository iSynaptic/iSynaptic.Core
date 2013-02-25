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
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons;
using iSynaptic.Commons.Text;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public class AggregateCodeAuthoringVisitor : DomainCodeAuthoringVisitor<string>
    {
        public AggregateCodeAuthoringVisitor(IndentingTextWriter writer, SymbolTable symbolTable) 
            : base(writer, symbolTable)
        {
        }

        protected void Visit(AggregateSyntax aggregate)
        {
            var genericIdType = aggregate.Identifier
                                         .OfType<GenericAggregateIdentifierSyntax>()
                                         .Select(x => x.Name)
                                         .OfType<NameSyntax>();

            var baseAggregate = aggregate.Base
                                         .Select(x => SymbolTable.Resolve(aggregate, x))
                                         .Select(x => x.Symbol)
                                         .Cast<AggregateSyntax>();

            var genericSuffix = genericIdType
                .Select(n => String.Format("<{0}>", n))
                .ValueOrDefault("");

            var baseName = aggregate.Base
                                    .Select(x => x.ToString())
                                    .ValueOrDefault("Aggregate");

            var idType = aggregate.Identifier
                                  .Cast<NamedAggregateIdentifierSyntax>()
                                  .Select(x => x.Type.Name);

            var baseGenericSuffix = genericIdType
                .Or(idType)
                .Where(x => !baseAggregate.HasValue ||
                            baseAggregate.SelectMaybe(y => y.Identifier)
                                         .Select(y => y is GenericAggregateIdentifierSyntax)
                                         .ValueOrDefault())
                .Select(x => String.Format("<{0}>", x))
                .ValueOrDefault("");

            WriteLine("public {0} partial class {1}{2} : {3}{4}",
                       aggregate.IsAbstract ? "abstract " : "",
                       aggregate.Name,
                       genericSuffix, 
                       baseName, 
                       baseGenericSuffix);

            if (genericIdType.HasValue)
            {
                using (WithIndentation())
                {
                    WriteLine("where {0} : IEquatable<{0}>", genericIdType.Value);
                }
            }

            using (WithBlock())
            {
                var id = aggregate.GetIdTypeName(SymbolTable);

                if(baseAggregate.HasValue)
                    WriteLine("protected {0} (AggregateEvent<{1}> startEvent) : base(startEvent) {{ }}", aggregate.Name, id);
                else
                    WriteLine("protected {0} (AggregateEvent<{1}> startEvent) {{ ApplyEvent(startEvent); }}", aggregate.Name, id);

                new AggregateEventCodeAuthoringVisitor(Writer, SymbolTable).Dispatch(aggregate.Members);
                new AggregateSnapshotCodeAuthoringVisitor(Writer, SymbolTable).Dispatch(aggregate.Members);
            }
        }
    }
}