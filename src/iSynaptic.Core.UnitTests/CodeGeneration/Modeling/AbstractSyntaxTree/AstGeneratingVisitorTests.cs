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
using System.Linq;
using NUnit.Framework;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree
{
    [TestFixture]
    public class AstGeneratingVisitorTests
    {
        [Test]
        public void CanGenerateAst()
        {
            var visitor = new AstGeneratingVisitor(Console.Out);

            var family = 
                Syntax.Family("iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree", new[]
                {
                    Syntax.Node("Family", "AstNodeFamily", true, Maybe.NoValue, Enumerable.Empty<String>(), new[]
                    {
                        Syntax.Property("Namespace", "String", false, false),
                        Syntax.Property("Nodes", "AstNode", true, true),
                    }),

                    Syntax.Node("Node", "AstNode", true, "AstNodeFamily".ToMaybe(), Enumerable.Empty<String>(), new[]
                    {
                        Syntax.Property("Name", "String", false, false),
                        Syntax.Property("TypeName", "String", false, false),
                        Syntax.Property("IsVisitable", "Boolean", false, false),
                        Syntax.Property("ParentType", "Maybe<String>", false, false),
                        Syntax.Property("BaseTypes", "String", false, true),
                        Syntax.Property("Properties", "AstNodeProperty", true, true)
                    }),

                    Syntax.Node("Property", "AstNodeProperty", true, "AstNode".ToMaybe(), Enumerable.Empty<String>(), new[]
                    {
                        Syntax.Property("Name", "String", false, false),
                        Syntax.Property("Type", "String", false, false),
                        Syntax.Property("IsNode", "Boolean", false, false),
                        Syntax.Property("IsMany", "Boolean", false, false)
                    }),
                });

            visitor.Dispatch(family);
        }
    }
}
