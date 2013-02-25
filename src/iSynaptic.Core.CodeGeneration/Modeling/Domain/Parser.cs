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
using Sprache;

using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    [CLSCompliant(false)]
    public abstract class Parser : StandardLanguageParser
    {
        public static readonly Parser<IdentifierNameSyntax> IdentifierName
            = from id in IdentifierOrKeyword
              select Syntax.IdentifierName(id);


        private static readonly Parser<IEnumerable<SimpleNameSyntax>> NamesParser
            = Parse.Ref(() => SimpleName).Delimit(".");

        public static readonly Parser<NameSyntax> Name = input =>
        {
            var names = NamesParser(input);
            if (names.WasSuccessful)
            {
                NameSyntax current = null;
                foreach (var name in names.Value)
                {
                    if (current != null)
                        current = Syntax.QualifiedName(current, name);
                    else
                        current = name;
                }

                return Sprache.Result.Success(current, names.Remainder);
            }

            return Sprache.Result.Failure<QualifiedNameSyntax>(names.Remainder, names.Message, names.Expectations);
        };

        public static readonly Parser<GenericNameSyntax> GenericName
            = from id in IdentifierOrKeyword
              from names in Name.Delimit(',').Surround('<', '>')
              select Syntax.GenericName(names, id);

        public static readonly Parser<SimpleNameSyntax> SimpleName
            = GenericName
            .Or<SimpleNameSyntax>(IdentifierName);

        public static readonly Parser<UsingStatementSyntax> UsingStatement
            = from keyword in Parse.String("using")
              from ns in Name
              from end in StatementEnd
              select Syntax.UsingStatement(ns);

        public static readonly Parser<TypeCardinalitySyntax> TypeCardinality
            = Parse.Char('*').Select(x => new TypeCardinalitySyntax(0))
            .Or(Parse.Char('+').Select(x => new TypeCardinalitySyntax(1)))
            .Or(Parse.Char('?').Select(x => new TypeCardinalitySyntax(0, 1)))
            .Or(Parse.Return(new TypeCardinalitySyntax(1, 1)));

        public static readonly Parser<TypeReferenceSyntax> TypeReference
            = from name in Name
              from cardinality in TypeCardinality
              select new TypeReferenceSyntax(name, cardinality);

        public static readonly Parser<AtomSyntax> Atom
            = from type in TypeReference
              from name in SimpleName
              from end in StatementEnd
              select Syntax.Atom(name, type);

        public static readonly Parser<EnumValueSyntax> EnumValue
            = IdentifierName.Select(Syntax.EnumValue);

        public static readonly Parser<EnumSyntax> Enum
            = from keyword in Parse.String("enum")
              from name in SimpleName
              from values in Blocked(EnumValue.Delimit(",").Optional())
              select Syntax.Enum(name, values);

        public static readonly Parser<ValueSyntax> Value
            = from isAbstract in Flag("abstract")
              from keyword in Parse.String("value")
              from name in SimpleName
              from @base in InheritsFrom(Name).Optional()
              from atoms in Blocked(Atom.Many())
              select Syntax.Value(isAbstract, name, @base, atoms);

        public static readonly Parser<Maybe<TypeReferenceSyntax>> AggregateIdentifierConstraint
            = (from type in Name
              let reference = new TypeReferenceSyntax(type, new TypeCardinalitySyntax(1, 1))
              select reference.ToMaybe())
              .Or(from _ in Parse.Char('*')
                  select Maybe<TypeReferenceSyntax>.NoValue);

        public static readonly Parser<AggregateIdentifierSyntax> AggregateIdentifier
            = (from name in IdentifierName
              from constraint in InheritsFrom(AggregateIdentifierConstraint)
              select Syntax.GenericAggregateIdentifier(name, constraint))
              .Or<AggregateIdentifierSyntax>(TypeReference.Select(Syntax.NamedAggregateIdentifier));

        public static readonly Parser<AggregateEventSyntax> AggregateEvent
            = from isAbstract in Flag("abstract")
              from keyword in Parse.String("event")
              from name in IdentifierName
              from @base in InheritsFrom(Name).Optional()
              from atoms in Blocked(Atom.Many())
              select Syntax.AggregateEvent(isAbstract, name, @base, atoms);

        public static readonly Parser<AggregateSyntax> Aggregate
            = from keyword in Parse.String("aggregate")
              from identifier in AggregateIdentifier.Surround('<', '>').Optional()
              from name in SimpleName
              from baseAggregate in InheritsFrom(Name).Optional()
              from events in Blocked(AggregateEvent.Many())
              select Syntax.Aggregate(name, identifier, baseAggregate, events);

        public static readonly Parser<NamespaceSyntax> Namespace
            = from keyword in Parse.String("namespace")
              from ns in Name
              from blockStart in BlockStart
              from usings in UsingStatement.Many()
              from members in Parse.Ref(() => NamespaceMember).Many()
              from blockEnd in BlockEnd
              select Syntax.Namespace(ns, usings, members);

        public static readonly Parser<INamespaceMember> NamespaceMember = ((Parser<INamespaceMember>)
                    Namespace)
                .Or(Enum)
                .Or(Value)
                .Or(Aggregate);

        public static readonly Parser<SyntaxTree> SyntaxTree
            = from usings in UsingStatement.Many()
              from namespaces in Namespace.Many()
              select Syntax.SyntaxTree(usings, namespaces);
    }
}
