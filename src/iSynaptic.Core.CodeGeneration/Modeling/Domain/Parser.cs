﻿// The MIT License
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

        public static readonly Parser<Visibility> VisibilityKeyword
            = Parse.String("public").Select(x => Visibility.Public)
            .Or(Parse.String("protected").Select(x => Visibility.Protected))
            .Or(Parse.String("internal").Select(x => Visibility.Internal))
            .Or(Parse.String("private").Select(x => Visibility.Private));

        public static readonly Parser<TypeReferenceSyntax> TypeReference
            = from name in Name
              from cardinality in TypeCardinality
              select new TypeReferenceSyntax(name, cardinality);

        public static readonly Parser<AnnotationPairSyntax> AnnotationPair
            = from name in IdentifierName
              from op in Parse.Char('=')
              from @string in Parse.CharExcept('"').Many().Text().Surround('"', '"')
              select Syntax.AnnotationPair(name, @string);

        public static readonly Parser<AnnotationSyntax> Annotation
            = from openBracket in Parse.Char('[')
              from name in IdentifierName
              from pairs in AnnotationPair.Delimit(',').Surround('(', ')').Optional()
              from closedBracket in Parse.Char(']')
              select Syntax.Annotation(name, pairs);

        public static readonly Parser<AtomSyntax> Atom
            = from annotations in Annotation.Many()
              from type in TypeReference
              from name in SimpleName
              from end in StatementEnd
              select Syntax.Atom(name, type, annotations);

        public static readonly Parser<EnumValueSyntax> EnumValue
            = IdentifierName.Select(Syntax.EnumValue);

        public static readonly Parser<EnumSyntax> Enum
            = from annotations in Annotation.Many()
              from isExternal in Flag("external")
              from visibility in VisibilityKeyword.Or(Parse.Return(Visibility.Public))
              from keyword in Parse.String("enum")
              from name in SimpleName
              from values in Blocked(EnumValue.Delimit(",").Optional()).Or(StatementEnd.Select(x => Enumerable.Empty<EnumValueSyntax>()))
              select Syntax.Enum(isExternal, visibility, name, values, annotations);

        public static readonly Parser<ScalarValueSyntax> ScalarValue
            = from annotations in Annotation.Many()
              from isExternal in Flag("external")
              from visibility in VisibilityKeyword.Or(Parse.Return(Visibility.Public))
              from isAbstract in Flag("abstract")
              from isPartial in Flag("partial")
              from keyword in Parse.String("value")
              from name in SimpleName
              from @base in InheritsFrom(Name)
              from end in StatementEnd
              select Syntax.ScalarValue(isExternal, visibility, isAbstract, isPartial, name, @base, annotations);

        public static readonly Parser<ValueSyntax> Value
            = from annotations in Annotation.Many()
              from isExternal in Flag("external")
              from visibility in VisibilityKeyword.Or(Parse.Return(Visibility.Public))
              from isAbstract in Flag("abstract")
              from isPartial in Flag("partial")
              from keyword in Parse.String("value")
              from name in SimpleName
              from @base in InheritsFrom(Name).Optional()
              from atoms in Blocked(Atom.Many()).Or(StatementEnd.Select(x => Enumerable.Empty<AtomSyntax>()))
              select Syntax.Value(isExternal, visibility, isAbstract, isPartial, name, @base, atoms, annotations);

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
            = from annotations in Annotation.Many()
              from isAbstract in Flag("abstract")
              from isPartial in Flag("partial")
              from keyword in Parse.String("event")
              from name in IdentifierName
              from @base in InheritsFrom(Name).Optional()
              from atoms in Blocked(Atom.Many()).Or(StatementEnd.Select(x => Enumerable.Empty<AtomSyntax>()))
              select Syntax.AggregateEvent(false, Visibility.Public, isAbstract, isPartial, name, @base, atoms, annotations);

        public static readonly Parser<AggregateSnapshotSyntax> AggregateSnapshot
            = from annotations in Annotation.Many()
              from isAbstract in Flag("abstract")
              from isPartial in Flag("partial")
              from keyword in Parse.String("snapshot")
              from name in IdentifierName
              from @base in InheritsFrom(Name).Optional()
              from atoms in Blocked(Atom.Many())
              select Syntax.AggregateSnapshot(false, Visibility.Public, isAbstract,isPartial, name, @base, atoms, annotations);

        public static readonly Parser<IAggregateMember> AggregateMember
            = AggregateEvent
            .Or<IAggregateMember>(AggregateSnapshot)
            .Or(ScalarValue)
            .Or(Value)
            .Or(Enum);

        public static readonly Parser<AggregateSyntax> Aggregate
            = from annotations in Annotation.Many()
              from isAbstract in Flag("abstract")
              from keyword in Parse.String("aggregate")
              from identifier in AggregateIdentifier.Surround('<', '>').Optional()
              from name in SimpleName
              from baseAggregate in InheritsFrom(Name).Optional()
              from members in Blocked(AggregateMember.Many()).Or(StatementEnd.Select(x => Enumerable.Empty<IAggregateMember>()))
              select Syntax.Aggregate(false, isAbstract, name, identifier, baseAggregate, members, annotations);

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
                .Or(ScalarValue)
                .Or(Value)
                .Or(Aggregate);

        public static readonly Parser<SyntaxTree> SyntaxTree
            = from usings in UsingStatement.Many()
              from namespaces in Namespace.Many()
              select Syntax.SyntaxTree(usings, namespaces);
    }
}
