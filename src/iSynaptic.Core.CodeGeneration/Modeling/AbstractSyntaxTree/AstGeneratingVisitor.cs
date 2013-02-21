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
using System.IO;
using System.Linq;
using iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree.SyntacticModel;
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;
using iSynaptic.Commons.Collections.Generic;

namespace iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree
{
    public class AstGeneratingVisitor : CSharpCodeAuthoringVisitor<String>
    {
        private readonly Dictionary<String, IAstConcept> _symbolTable;
        private readonly Func<String, IVisitable, IVisitable, String> _lineInterleave;

        public AstGeneratingVisitor(TextWriter writer, Dictionary<String, IAstConcept> symbolTable) : base(writer)
        {
            _symbolTable = Guard.NotNull(symbolTable, "symbolTable");
            _lineInterleave = (st, l, r) => { WriteLine(); return st; };
        }

        public Maybe<IAstConcept> Resolve(IAstConcept relativeTo, String nodeType)
        {
            return nodeType != relativeTo.TypeName 
                ? _symbolTable.TryGetValue(String.Format("{0}.{1}", relativeTo.Parent.Namespace, nodeType)) 
                : relativeTo.ToMaybe();
        }

        protected String Visit(AstNodeFamily family, String mode)
        {
            WriteLine("using System;");
            WriteLine("using System.Collections.Generic;");
            WriteLine("using System.Linq;");
            WriteLine("using iSynaptic.Commons;");

            WriteLine();

            using (WriteBlock("namespace {0}", family.Namespace))
            {
                WriteLine("using SyntacticModel;");
                WriteLine();

                WriteSyntaxClass(family);
            }

            WriteLine();

            using (WriteBlock("namespace {0}.SyntacticModel", family.Namespace))
            {
                WriteLine("internal interface IAstNode<out T> : IVisitable { T GetUnderlying(); }");
                WriteLine();

                WriteLine("internal interface IAstUnderlyingNode<out T, in TParent> { T MakePublic(TParent parent); }");
                WriteLine();


                DispatchChildren(family, "public", _lineInterleave);

                WriteLine();
                using (WriteBlock("namespace Internal"))
                {
                    DispatchChildren(family, "internal", _lineInterleave);
                }
            }

            return null;
        }

        private void WriteSyntaxClass(AstNodeFamily family)
        {
            using (WriteBlock("public static class Syntax"))
            {
                DispatchChildren(family, "syntaxMethod", _lineInterleave);
            }
        }

        protected String Visit(AstNodeContract contract, String mode)
        {
            var baseTypes = contract.BaseTypes.ToArray();

            if (mode == "public")
            {
                var publicBaseTypes = baseTypes.Concat(new[] {"IVisitable"}).Delimit(", ");

                using (WriteBlock("public interface {0} : {1}", contract.TypeName, publicBaseTypes))
                {
                    DispatchChildren(contract, "contractProperty");

                    if (contract.ParentType.HasValue)
                    {
                        WriteLine("{0} Parent {{ get; }}", contract.ParentType.Value);
                    }
                }
            }

            if (mode == "internal")
            {
                var internalBaseTypes = baseTypes.Any()
                    ? String.Format(" : {0}", baseTypes.Delimit(", "))
                    : "";

                using (WriteBlock("internal interface {0}{1}", contract.TypeName, internalBaseTypes))
                {
                }
            }

            return mode;
        }

        protected String Visit(AstNode node, String mode)
        {
            var nodeHierarcy = node.Recurse(x => x.BaseTypes.TryFirst().SelectMaybe(y => Resolve(x, y).OfType<AstNode>())).ToArray();

            var baseNode = nodeHierarcy.Skip(1).TryFirst();
            var parentNode = nodeHierarcy
                .SelectMaybe(x => x.ParentType)
                .SelectMaybe(x => Resolve(node, x))
                .TryFirst();

            var lowestBaseNode = nodeHierarcy.Last();
            var lowestParentNode = nodeHierarcy
                .SelectMaybe(x => x.ParentType)
                .SelectMaybe(x => Resolve(node, x))
                .TryLast();


            var baseTypes = node.BaseTypes.Concat(new[]{String.Format("IAstNode<Internal.{0}>", node.TypeName)});

            if (mode == "public")
            {
                String baseTypesSuffix = String.Format(" : {0}", baseTypes.Delimit(", "));

                using (WriteBlock("public {0}partial class {1}{2}", node.IsAbstract ? "abstract " : "", node.TypeName, baseTypesSuffix))
                {
                    if (parentNode.HasValue)
                        WriteLine("private readonly {0} _parent;", parentNode.Value.TypeName);

                    WriteLine("private readonly Internal.{0} _underlying;", node.TypeName);

                    WriteLine();

                    string ctor = parentNode.HasValue
                        ? "internal {1}({0} parent, Internal.{1} underlying)"
                        : "internal {1}(Internal.{1} underlying)";

                    if (baseNode.HasValue)
                        ctor = String.Format("{0} : base({1})", ctor, parentNode.HasValue ? "parent, underlying" : "underlying");

                    using (WriteBlock(ctor, parentNode.Select(x => x.TypeName).ValueOrDefault(), node.TypeName))
                    {
                        if (parentNode.HasValue)
                            WriteLine("_parent = parent;");

                        WriteLine("_underlying = underlying;");
                    }

                    WriteLine();

                    if (parentNode.HasValue)
                    {
                        if(lowestParentNode.HasValue && parentNode.Value != lowestParentNode.Value)
                            WriteLine("public new {0} Parent {{ get {{ return _parent; }} }}", parentNode.Value.TypeName);
                        else
                            WriteLine("public {0} Parent {{ get {{ return _parent; }} }}", parentNode.Value.TypeName);
                    }

                    WriteLine("Internal.{0} IAstNode<Internal.{0}>.GetUnderlying() {{ return _underlying; }}", node.TypeName);
                    WriteLine();

                    String acceptChildrenModifier = baseNode.HasValue ? "override" : "virtual";

                    Boolean hasNodeProperties = node.Properties.SelectMaybe(x => Resolve(node, x.Type)).Any();

                    if (!baseNode.HasValue || hasNodeProperties)
                    {
                        using (WriteBlock("public {0} void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)", acceptChildrenModifier))
                        {
                            DispatchChildren(node, "dispatchInvoke");

                            if (baseNode.HasValue)
                                WriteLine("base.AcceptChildren(dispatch);");
                        }
                        WriteLine();
                    }

                    DispatchChildren(node, "publicSelector");
                }
            }

            if (mode == "internal")
            {
                String parentType = parentNode.HasValue
                    ? String.Format("SyntacticModel.{0}", parentNode.Value.TypeName)
                    : "Object";

                var internalBaseTypes = node.BaseTypes
                    .Where(x => Resolve(node, x).HasValue)
                    .Concat(new[] { String.Format("IAstUnderlyingNode<SyntacticModel.{0}, {1}>", node.TypeName, parentType)});

                using (WriteBlock("internal {0}class {1} : {2}",
                    node.IsAbstract ? "abstract " : "", 
                    node.TypeName,
                    internalBaseTypes.Delimit(", ")))
                {
                    DispatchChildren(node, "field");
                    WriteLine();

                    var needsAnyParameters = nodeHierarcy.SelectMany(x => x.Properties).Any();
                    var baseNeedsAnyArguments = nodeHierarcy.Skip(1).SelectMany(x => x.Properties).Any();

                    if (needsAnyParameters)
                    {
                        Write("{0} {1}(", node.IsAbstract ? "protected" : "public", node.TypeName);
                        DispatchChildren(nodeHierarcy, "parameter", (s, l, r) => { Write(", "); return s; });
                        Write(")");

                        if (baseNode.HasValue && baseNeedsAnyArguments)
                        {
                            Write(": base(");
                            DispatchChildren(nodeHierarcy.Skip(1), "argument", (s, l, r) => { Write(", "); return s; });
                            Write(")");
                        }

                        WriteLine();
                        using (WithBlock())
                        {
                            DispatchChildren(node, "assignArgument");
                        }
                        WriteLine();
                    }

                    String makePublicInheritanceModifier = lowestBaseNode == node ? "" : "new ";
                    using (WriteBlock("public {0}SyntacticModel.{1} MakePublic({2} parent)", makePublicInheritanceModifier, node.TypeName, parentType))
                    {
                        WriteLine(lowestBaseNode == node
                                    ? "return BuildPublic(parent);"
                                    : "return (SyntacticModel.{0}) BuildPublic(parent);", 
                                  node.TypeName);
                    }
                    WriteLine();

                    String buildParentParameter = lowestParentNode.HasValue
                        ? String.Format("SyntacticModel.{0} parent", lowestParentNode.Value.TypeName)
                        : "Object parent";

                    String buildParentArgument = "";

                    if (parentNode.HasValue)
                    {
                        buildParentArgument = parentNode.Value != lowestParentNode.Value 
                            ? String.Format("(SyntacticModel.{0}) parent, ", parentNode.Value.TypeName) 
                            : "parent, ";
                    }

                    if (node.IsAbstract && lowestBaseNode == node)
                    {
                        WriteLine("protected abstract SyntacticModel.{0} BuildPublic({1});", lowestBaseNode.TypeName, buildParentParameter);
                    }
                    else if (!node.IsAbstract)
                    {
                        String buildPublicInheritanceModifier = lowestBaseNode == node ? "virtual" : "override";
                        using (WriteBlock("protected {0} SyntacticModel.{1} BuildPublic({2})", buildPublicInheritanceModifier, lowestBaseNode.TypeName, buildParentParameter))
                        {
                            WriteLine("return new SyntacticModel.{0}({1} this);", node.TypeName, buildParentArgument);
                        }
                        WriteLine();
                    }

                    DispatchChildren(node, "property");
                }
            }

            if (mode == "syntaxMethod" && !node.IsAbstract)
            {
                Write("public static {0} {1}(", node.TypeName, node.Name);
                DispatchChildren(nodeHierarcy, "parameter", (s, l, r) => { Write(", "); return s; });
                using(WriteBlock(")"))
                {

                    Write(parentNode.HasValue
                            ? "return new {0}(null, new SyntacticModel.Internal.{0}("
                            : "return new {0}(new SyntacticModel.Internal.{0}(",
                        node.TypeName,
                        parentNode.Select(x => x.TypeName).ValueOrDefault());
                    DispatchChildren(nodeHierarcy, "argumentUnderlyingSelector", (s, l, r) => { Write(", "); return s; });
                    WriteLine("));");
                }
            }

            return mode;
        }

        protected String Visit(AstNodeProperty property, String mode)
        {
            var node = Resolve(property.Parent, property.Type);
            Boolean isNodeProperty = node.HasValue;

            if (mode == "field")
            {
                WriteLine("private readonly {0} _{1};",
                          GetPropertyType(property),
                          Camelize(property.Name));
            }

            if (mode == "parameter")
            {
                Write("{0} {1}", 
                      GetPropertyType(property), 
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
                WriteLine("public {0} {1} {{ get {{ return _{2}; }} }}",
                          GetPropertyType(property), 
                          property.Name, 
                          Camelize(property.Name));
            }

            if (mode == "contractProperty")
            {
                WriteLine("{0} {1} {{ get; }}",
                          GetPropertyType(property),
                          property.Name);
            }

            if (mode == "publicSelector")
            {
                using (WriteBlock("public {0} {1}", GetPropertyType(property), property.Name))
                using (WriteBlock("get"))
                {
                    if (isNodeProperty)
                    {
                        WriteLine(property.Cardinality != AstNodePropertyCardinality.One
                                    ? "return ((IAstNode<Internal.{0}>)this).GetUnderlying().{1}.Select(x => ((IAstUnderlyingNode<{2}, {0}>)x).MakePublic(this));"
                                    : "return ((IAstUnderlyingNode<{2}, {0}>)((IAstNode<Internal.{0}>)this).GetUnderlying().{1}).MakePublic(this);",
                                  property.Parent.TypeName, property.Name, property.Type);
                    }
                    else
                    {
                        WriteLine("return ((IAstNode<Internal.{0}>)this).GetUnderlying().{1};", property.Parent.TypeName, property.Name);
                    }
                }
            }

            if (mode == "argumentUnderlyingSelector")
            {
                if (isNodeProperty)
                {
                    Write(property.Cardinality != AstNodePropertyCardinality.One
                              ? "{0}.Select(x => ((IAstNode<SyntacticModel.Internal.{1}>)x).GetUnderlying())"
                              : "((IAstNode<SyntacticModel.Internal.{1}>){0}).GetUnderlying()",
                          SafeIdentifier(Camelize(property.Name)),
                          property.Type);
                }
                else
                    Write(SafeIdentifier(Camelize(property.Name)));
            }

            if (mode == "dispatchInvoke" && isNodeProperty)
            {
                if(property.Cardinality == AstNodePropertyCardinality.One)
                    WriteLine("dispatch(new[]{{ {0} }});", property.Name);

                if (property.Cardinality == AstNodePropertyCardinality.ZeroOrOne)
                    WriteLine("dispatch({0}.ToEnumerable());", property.Name);

                if(property.Cardinality == AstNodePropertyCardinality.Many)
                    WriteLine("dispatch({0});", property.Name);
            }

            return mode;
        }

        private String GetPropertyType(AstNodeProperty property)
        {
            if (property.Cardinality == AstNodePropertyCardinality.One)
                return property.Type;

            if (property.Cardinality == AstNodePropertyCardinality.ZeroOrOne)
                return String.Format("Maybe<{0}>", property.Type);

            if(property.Cardinality == AstNodePropertyCardinality.Many)
                return String.Format("IEnumerable<{0}>", property.Type);

            throw new InvalidOperationException("Unrecognized cardinality.");
        }
    }
}
