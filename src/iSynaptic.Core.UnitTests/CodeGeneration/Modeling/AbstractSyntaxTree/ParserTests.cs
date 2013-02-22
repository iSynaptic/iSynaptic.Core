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

using FluentAssertions;
using NUnit.Framework;
using Sprache;

namespace iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Parse_AstFamily()
        {
            Parser.ParseString(@"ast iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree
{
  node AstNode(""Node"", AstNodeFamily)
  {
    String SimpleName;
    String TypeName;
    String? ParentType;
    String* BaseTypes;
    AstNodeProperty* Properties;
  }

  node AstNodeFamily(""Family"")
  {
    String Namespace;
    AstNode* Nodes;
  }

  node AstNodeProperty(""Property"", AstNode)
  {
    String SimpleName;
    String Type;
    Boolean IsNode;
    AstNodePropertyCardinality Cardinality;
  }
}");
        }

        [Test]
        public void Parse_Node()
        {
            var parser = Parser.Node();

            parser.Parse(@"node AstNode(""Node"", AstNodeFamily)
  {
    String SimpleName;
    String TypeName;
    String? ParentType;
    String* BaseTypes;
    AstNodeProperty* Properties;
  }");
        }

        [Test]
        public void Property_WithNoCardinalityModifier_Parsers()
        {
            var parser = Parser.Property();
            var results = parser.Parse("String SimpleName;");

            results.Type.Should().Be("String");
            results.Cardinality.Should().Be(AstNodePropertyCardinality.One);
            results.SimpleName.Should().Be("SimpleName");
        }

        [Test]
        public void Property_WithZeroOrOneModifier_Parsers()
        {
            var parser = Parser.Property();
            var results = parser.Parse("String? SimpleName;");

            results.Type.Should().Be("String");
            results.Cardinality.Should().Be(AstNodePropertyCardinality.ZeroOrOne);
            results.SimpleName.Should().Be("SimpleName");
        }

        [Test]
        public void Property_WithManyModifier_Parsers()
        {
            var parser = Parser.Property();
            var results = parser.Parse("String* SimpleName;");

            results.Type.Should().Be("String");
            results.Cardinality.Should().Be(AstNodePropertyCardinality.Many);
            results.SimpleName.Should().Be("SimpleName");
        }
    }
}
