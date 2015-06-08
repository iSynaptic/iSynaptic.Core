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
using System.Linq;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public class ApplyAggregateEventCodeAuthoringVisitor : MoleculeCodeAuthoringVisitor
    {
        public ApplyAggregateEventCodeAuthoringVisitor(IndentingTextWriter writer, SymbolTable symbolTable, DomainCodeAuthoringSettings settings) : base(writer, symbolTable, settings)
        {
        }

        protected override bool NotInterestedIn(object subject, string state)
        {
            return subject is MoleculeSyntax && !(subject is AggregateEventSyntax);
        }

        protected override void Visit(MoleculeSyntax molecule)
        {
            if (molecule.IsAbstract) return;

            var atoms = molecule.Atoms.Select(GetAtomInfo).Concat(GetBaseAtomInfo(molecule));

            Write($"protected void Apply{molecule.Name}(");
            Delimit(atoms, "parameter", ", ");
            WriteLine(")");
            using (WithBlock())
            {
                WriteLine($"if (Version <= 0) throw new InvalidOperationException(\"This overload of Apply{molecule.Name} can only be called after the first event is applied. Please call the overload which requires the Aggregate's ID to be provided.\");");
                WriteLine();

                Write($"ApplyEvent(new {molecule.Name}(");
                Delimit(atoms, "argument", ", ");
                WriteLine($"{(atoms.Any() ? ", " : "")}Id, Version + 1));");
            }

            var aggregate = (AggregateSyntax)molecule.Parent;

            var id = aggregate.GetIdTypeName(SymbolTable);
            var idAtom = new AtomInfo(Syntax.IdentifierName("id"), new TypeReferenceSyntax(id, new TypeCardinalitySyntax(1, 1)), true);

            atoms = atoms.Concat(new[] { idAtom });

            Write($"protected void Apply{molecule.Name}(");
            Delimit(atoms, "parameter", ", ");
            WriteLine(")");
            using (WithBlock())
            {
                Write($"ApplyEvent(new {molecule.Name}(");
                Delimit(atoms, "argument", ", ");
                WriteLine($", Version + 1));");

            }
        }
    }
}
