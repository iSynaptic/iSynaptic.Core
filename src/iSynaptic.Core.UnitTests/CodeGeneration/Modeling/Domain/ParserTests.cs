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

using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Sprache;
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void IdentifierName_Parses()
        {
            var name = Parser.IdentifierName.Parse("AnIdentifier");
            name.Should().NotBeNull();
            name.Identifier.Should().Be("AnIdentifier");

            name.ToString().Should().Be("AnIdentifier");
        }

        [Test]
        public void GenericName_Parses()
        {
            var name = Parser.GenericName.Parse("AGeneric<Left,Right>");
            name.Should().NotBeNull();
            name.Identifier.Should().Be("AGeneric");
            name.TypeArguments
                .Select(x => x.ToString())
                .SequenceEqual(new[] {"Left", "Right"});

            name.ToString().Should().Be("AGeneric<Left, Right>");
        }

        [Test]
        public void SimpleName_Parses()
        {
            var identifierName = Parser.SimpleName.Parse("AnIdentifier") as IdentifierNameSyntax;
            identifierName.Should().NotBeNull();
            identifierName.Identifier.Should().Be("AnIdentifier");

            identifierName.ToString().Should().Be("AnIdentifier");


            var genericName = Parser.SimpleName.Parse("AGeneric<Left,Right>") as GenericNameSyntax;

            genericName.Should().NotBeNull();
            genericName.Identifier.Should().Be("AGeneric");
            genericName.TypeArguments
                .Select(x => x.ToString())
                .SequenceEqual(new[] { "Left", "Right" });

            genericName.ToString().Should().Be("AGeneric<Left, Right>");
        }

        [Test]
        public void Name_ParsesSimpleName()
        {
            var identifierName = Parser.Name.Parse("AnIdentifier") as IdentifierNameSyntax;
            identifierName.Should().NotBeNull();
            identifierName.Identifier.Should().Be("AnIdentifier");

            identifierName.ToString().Should().Be("AnIdentifier");


            var genericName = Parser.Name.Parse("AGeneric<Left,Right>") as GenericNameSyntax;

            genericName.Should().NotBeNull();
            genericName.Identifier.Should().Be("AGeneric");
            genericName.TypeArguments
                .Select(x => x.ToString())
                .SequenceEqual(new[] { "Left", "Right" });

            genericName.ToString().Should().Be("AGeneric<Left, Right>");

        }

        [Test]
        public void Name_ParsesQualifiedName()
        {
            var qualifiedName = Parser.Name.Parse("Foo . Bar < Baz > . Quix") as QualifiedNameSyntax;
            qualifiedName.Should().NotBeNull();

            qualifiedName.ToString().Should().Be("Foo.Bar<Baz>.Quix");
        }

        [Test]
        public void Enum_ParsesExternalEnum()
        {
            var @enum = Parser.Enum.Parse("external enum TimeOfDay { }");
            @enum.Should().NotBeNull();
            @enum.IsExternal.Should().BeTrue();
            @enum.Name.ToString().Should().Be("TimeOfDay");
        }
    }
}
