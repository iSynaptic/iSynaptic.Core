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
using NUnit.Framework;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling
{
    //[TestFixture]
    //public class AstGeneratingVisitorTests
    //{
    //    [Test]
    //    public void CanGenerateAst()
    //    {
    //        var visitor = new AstGeneratingVisitor(Console.Out);

    //        var family = new AstNodeFamily("iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree",
    //            new AstNode
    //            {
    //                Name = "Family",
    //                TypeName = "AstNodeFamily",
    //                Properties = new[]
    //                {
    //                    new AstNodeProperty
    //                    {
    //                        Name = "Namespace",
    //                        Type = "String"
    //                    },
    //                    new AstNodeProperty
    //                    {
    //                        Name = "Nodes", 
    //                        Type = "AstNode", 
    //                        IsNode = true, 
    //                        IsMany = true
    //                    },
    //                }
    //            },
    //            new AstNode
    //            {
    //                Name = "Node",
    //                TypeName = "AstNode",
    //                ParentType = "AstNodeFamily".ToMaybe(),
    //                Properties = new []
    //                {
    //                    new AstNodeProperty
    //                    {
    //                        Name = "Name",
    //                        Type = "String"
    //                    },
    //                    new AstNodeProperty
    //                    {
    //                        Name = "TypeName",
    //                        Type = "String"
    //                    },
    //                    new AstNodeProperty
    //                    {
    //                        Name = "ParentType",
    //                        Type = "Maybe<String>"
    //                    },
    //                    new AstNodeProperty
    //                    {
    //                        Name = "Properties",
    //                        Type = "AstNodeProperty",
    //                        IsMany = true,
    //                        IsNode = true
    //                    },
    //                }
    //            },
    //            new AstNode
    //            {
    //                Name = "Property",
    //                TypeName = "AstNodeProperty",
    //                ParentType = "AstNode".ToMaybe(),
    //                Properties = new[]
    //                {
    //                    new AstNodeProperty
    //                    {
    //                        Name = "Name",
    //                        Type = "String"
    //                    },
    //                    new AstNodeProperty
    //                    {
    //                        Name = "Type",
    //                        Type = "String"
    //                    },
    //                    new AstNodeProperty
    //                    {
    //                        Name = "IsNode",
    //                        Type = "Boolean"
    //                    },
    //                    new AstNodeProperty
    //                    {
    //                        Name = "IsMany",
    //                        Type = "Boolean"
    //                    }
    //                }
    //            }
    //        );

    //        visitor.Dispatch(family);
    //    }
    //}
}
