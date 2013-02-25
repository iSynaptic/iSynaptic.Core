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
using Sprache;
using iSynaptic.Commons.Linq;

namespace iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree
{
    using SyntacticModel;

    [CLSCompliant(false)]
    public class Parser : StandardLanguageParser
    {
        private Parser() { }

        private struct TypeReference
        {
            public TypeReference(String name, AstNodePropertyCardinality cardinality)
                : this()
            {
                Name = name;
                Cardinality = cardinality;
            }

            public string Name { get; private set; }
            public AstNodePropertyCardinality Cardinality { get; private set; }
        }

        public static AstNodeFamily ParseString(String input)
        {
            return Family().Parse(input);
        }

        public static Parser<AstNodeFamily> Family()
        {
            return from keyword in Parse.String("ast")
                   from ns in NamespaceOrTypeName
                   from nodes in Blocked(Node().Or<IAstConcept>(Contract()).Many())
                   select Syntax.Family(ns, nodes);
        }

        public static Parser<AstNode> Node()
        {
            return from isAbstract in Flag("abstract")
                   from isPartial in Flag("partial")
                   from keyword in Parse.String("node")
                   from typeName in IdentifierOrKeyword
                   from startParen in Parse.Char('(')
                   from startQuote in Parse.Char('"')
                   from name in IdentifierOrKeyword
                   from endQuote in Parse.Char('"')
                   from parent in
                   (
                        from comma in Parse.Char(',')
                        from n in IdentifierOrKeyword
                        select n
                    ).Optional()

                   from endParen in Parse.Char(')')
                   from baseTypes in
                   (
                        from colun in Parse.Char(':')
                        from types in TypeName().Delimit(Parse.Char(','))
                        select types
                   ).Optional()
                   from blockStart in Parse.Char('{')
                   from properties in Property().Many()
                   from blockEnd in Parse.Char('}')
                   select Syntax.Node(
                            isAbstract,
                            isPartial,
                            name,
                            typeName,
                            parent,
                            baseTypes,
                            properties);
        }

        public static Parser<AstNodeContract> Contract()
        {
            return from keyword in Parse.String("contract")
                   from typeName in IdentifierOrKeyword
                   from parent in IdentifierOrKeyword.Surround('(', ')').Optional()
                   from baseTypes in
                       (
                            from colun in Parse.Char(':')
                            from types in TypeName().Delimit(Parse.Char(','))
                            select types
                       ).Optional()
                   from blockStart in Parse.Char('{')
                   from properties in Property().Many()
                   from blockEnd in Parse.Char('}')
                   select Syntax.Contract(
                            typeName,
                            parent,
                            baseTypes,
                            properties);

        }

        public static Parser<AstNodeProperty> Property()
        {
            return from type in TypeRef()
                   from name in IdentifierOrKeyword
                   from end in Parse.Char(';')
                   select Syntax.Property(name, type.Name, type.Cardinality);
        }

        private static Parser<TypeReference> TypeRef()
        {
            return from name in TypeName()
                   from cardinality in PropertyCardinalityModifier()
                   select new TypeReference(name, cardinality);
        }

        private static Parser<String> TypeName()
        {
            return (from outerType in IdentifierOrKeyword
                   from startAngle in Parse.Char('<')
                   from innerType in TypeName()
                   from endAngle in Parse.Char('>')
                   select String.Format("{0}<{1}>", outerType, innerType))
                   .Or(IdentifierOrKeyword);
        }

        private static Parser<AstNodePropertyCardinality> PropertyCardinalityModifier()
        {
            return Parse.Char('*').Select(x => AstNodePropertyCardinality.Many)
               .Or(Parse.Char('?').Select(x => AstNodePropertyCardinality.ZeroOrOne))
               .Or(Parse.Return(AstNodePropertyCardinality.One));
        }
    }
}
