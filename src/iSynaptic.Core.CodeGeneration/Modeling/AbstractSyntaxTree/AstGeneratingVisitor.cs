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
using System.IO;
using System.Linq;
using iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree.SyntacticModel;
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;

namespace iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree
{
    public class AstGeneratingVisitor : CSharpCodeAuthoringVisitor<String>
    {
        private readonly Func<String, IVisitable, IVisitable, String> lineInterleave;

        public AstGeneratingVisitor(TextWriter writer) : base(writer)
        {
            lineInterleave = (st, l, r) => { WriteLine(); return st; };
        }

        protected override void AddUsings()
        {
            base.AddUsings();
            AddUsing("iSynaptic.Commons");
        }

        protected String Visit(AstNodeFamily family, String mode)
        {
            WriteUsings();

            using (WriteBlock("namespace {0}", family.Namespace))
            {
                WriteLine("using SyntacticModel;");
                WriteLine();

                WriteSyntaxClass(family);
            }

            WriteLine();

            using (WriteBlock("namespace {0}.SyntacticModel", family.Namespace))
            {
                DispatchChildren(family, "public", lineInterleave);

                WriteLine();
                using (WriteBlock("namespace Internal"))
                {
                    DispatchChildren(family, "internal", lineInterleave);
                }
            }

            return null;
        }

        private void WriteSyntaxClass(AstNodeFamily family)
        {
            using (WriteBlock("public static class Syntax"))
            {
                DispatchChildren(family, "syntaxMethod", lineInterleave);
            }
        }

        protected String Visit(AstNode node, String mode)
        {
            if (mode == "public")
            {
                var baseTypes = node.BaseTypes;
                if (node.IsVisitable)
                    baseTypes = baseTypes.Concat(new[] {"IVisitable"});

                string baseTypesSuffix = baseTypes.Any()
                    ? String.Format(" : {0}", baseTypes.Delimit(", "))
                    : "";

                using (WriteBlock("public partial class {0}{1}", node.TypeName, baseTypesSuffix))
                {
                    if (node.ParentType.HasValue)
                        WriteLine("private readonly {0} _parent;", node.ParentType.Value);

                    WriteLine("private readonly Internal.{0} _underlying;", node.TypeName);
                    WriteLine();

                    string ctor = node.ParentType.HasValue
                        ? "internal {1}({0} parent, Internal.{1} underlying)"
                        : "internal {1}(Internal.{1} underlying)";

                    using (WriteBlock(ctor, node.ParentType.ValueOrDefault(), node.TypeName))
                    {
                        if (node.ParentType.HasValue)
                            WriteLine("_parent = parent;");

                        WriteLine("_underlying = underlying;");
                    }

                    WriteLine();

                    if(node.ParentType.HasValue)
                        WriteLine("public {0} Parent {{ get {{ return _parent; }} }}", node.ParentType.Value);

                    WriteLine("internal Internal.{0} GetUnderlying() {{ return _underlying; }}", node.TypeName);
                    WriteLine();

                    using (WriteBlock("public void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)"))
                    {
                        DispatchChildren(node, "dispatchInvoke");
                    }

                    DispatchChildren(node, "publicSelector");
                }
            }

            if (mode == "internal")
            {
                using (WriteBlock("internal class {0}", node.TypeName))
                {
                    DispatchChildren(node, "field");
                    WriteLine();

                    Write("public {0}(", node.TypeName);
                    DispatchChildren(node, "parameter", (s, l, r) => { Write(", "); return s; });
                    
                    using(WriteBlock(")"))
                    {
                        DispatchChildren(node, "assignArgument");
                    }

                    WriteLine();
                    DispatchChildren(node, "property");
                }
            }

            if (mode == "syntaxMethod")
            {
                Write("public static {0} {1}(", node.TypeName, node.Name);
                DispatchChildren(node, "parameter", (s, l, r) => { Write(", "); return s; });
                using(WriteBlock(")"))
                {

                    Write(node.ParentType.HasValue
                            ? "return new {0}(null, new SyntacticModel.Internal.{0}("
                            : "return new {0}(new SyntacticModel.Internal.{0}(",
                        node.TypeName, 
                        node.ParentType.ValueOrDefault());
                    DispatchChildren(node, "argumentUnderlyingSelector", (s, l, r) => { Write(", "); return s; });
                    WriteLine("));");
                }
            }

            return mode;
        }

        protected String Visit(AstNodeProperty property, String mode)
        {
            if (mode == "field")
            {
                WriteLine(property.IsMany 
                            ? "private readonly IEnumerable<{0}> _{1};" 
                            : "private readonly {0} _{1};",
                          property.Type,
                          Camelize(property.Name));
            }

            if (mode == "parameter")
            {
                Write(property.IsMany 
                        ? "IEnumerable<{0}> {1}"
                        : "{0} {1}", 
                      property.Type, 
                      SafeIdentifier(Camelize(property.Name)));
            }

            if (mode == "argument")
            {
                Write("{0}", SafeIdentifier(Camelize(property.Name)));
            }

            if (mode == "assignArgument")
            {
                WriteLine("_{0} = {1};", Camelize(property.Name), SafeIdentifier(Camelize(property.Name)));
            }

            if (mode == "property")
            {
                WriteLine(property.IsMany
                            ? "public IEnumerable<{0}> {1} {{ get {{ return _{2}; }} }}"
                            : "public {0} {1} {{ get {{ return _{2}; }} }}",
                          property.Type, 
                          property.Name, 
                          Camelize(property.Name));
            }

            if (mode == "publicSelector")
            {
                String type = String.Format(property.IsMany
                                                ? "IEnumerable<{0}>"
                                                : "{0}",
                                            property.Type);

                using (WriteBlock("public {0} {1}", type, property.Name))
                using (WriteBlock("get"))
                {
                    if (property.IsNode)
                    {
                        WriteLine(property.IsMany
                                    ? "return _underlying.{0}.Select(x => new {1}(this, x));"
                                    : "return new {1}(this, _underlying.{0});",
                                  property.Name, property.Type);
                    }
                    else
                    {
                        WriteLine("return _underlying.{0};", property.Name);
                    }
                }
            }

            if (mode == "argumentUnderlyingSelector")
            {
                if (property.IsNode)
                {
                    Write(property.IsMany
                              ? "{0}.Select(x => x.GetUnderlying())"
                              : "{0}.GetUnderlying()",
                          SafeIdentifier(Camelize(property.Name)));
                }
                else
                    Write(SafeIdentifier(Camelize(property.Name)));
            }

            if (mode == "dispatchInvoke" && property.IsNode)
            {
                WriteLine(property.IsMany
                            ? "dispatch({0});"
                            : "dispatch(new[]{{ {0} }})", 
                          property.Name);
            }

            return mode;
        }
    }
}
