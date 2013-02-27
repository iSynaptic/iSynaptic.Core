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
using System.Collections.Generic;
using System.Linq;
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons;
using iSynaptic.Commons.Text;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public class AggregateSnapshotCodeAuthoringVisitor : MoleculeCodeAuthoringVisitor
    {
        public AggregateSnapshotCodeAuthoringVisitor(IndentingTextWriter writer, SymbolTable symbolTable)
            : base(writer, symbolTable)
        {
        }

        protected override bool NotInterestedIn(Object subject, string state)
        {
            return subject is MoleculeSyntax && !(subject is AggregateSnapshotSyntax);
        }

        protected override IEnumerable<AtomInfo> GetBaseAtomInfo(MoleculeSyntax molecule, Maybe<MoleculeSyntax> baseValue)
        {
            var aggregate = (AggregateSyntax)molecule.Parent;

            var id = aggregate.GetIdTypeName(SymbolTable);

            return base.GetBaseAtomInfo(molecule, baseValue)
                .Concat(new[]
                {
                    new AtomInfo(Syntax.IdentifierName("id"), new TypeReferenceSyntax(id, new TypeCardinalitySyntax(1, 1)), true), 
                    new AtomInfo(Syntax.IdentifierName("version"), new TypeReferenceSyntax(Syntax.IdentifierName("Int32"), new TypeCardinalitySyntax(1, 1)), true),
                    new AtomInfo(Syntax.IdentifierName("takenAt"), new TypeReferenceSyntax(Syntax.IdentifierName("DateTime"), new TypeCardinalitySyntax(1, 1)), true)
                });
        }

        protected override Maybe<string> GetBaseMolecule(MoleculeSyntax molecule)
        {
            var aggregate = (AggregateSyntax)molecule.Parent;

            var id = aggregate.GetIdTypeName(SymbolTable);

            return base.GetBaseMolecule(molecule)
                       .Or(String.Format("AggregateSnapshot<{0}>", id));
        }

        protected override bool ShouldBeEquatable(MoleculeSyntax molecule)
        {
            return false;
        }
    }
}