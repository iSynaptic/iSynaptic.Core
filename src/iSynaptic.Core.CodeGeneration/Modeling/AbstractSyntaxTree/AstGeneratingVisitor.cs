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

        public AstGeneratingVisitor(TextWriter writer, Dictionary<String, IAstConcept> symbolTable)
            : base(writer)
        {
            _symbolTable = Guard.NotNull(symbolTable, "symbolTable");
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
                WriteLine("internal interface IAstNode<out T> : IVisitableChildren { T GetUnderlying(); }");
                WriteLine();

                WriteLine("internal interface IAstUnderlyingNode<out T, in TParent> { T MakePublic(TParent parent); }");
                WriteLine();

                Delimit(family.Concepts, "public", Environment.NewLine);

                WriteLine();
                using (WriteBlock("namespace Internal"))
                {
                    Delimit(family.Concepts, "internal", Environment.NewLine);
                }
            }

            return null;
        }

        private void WriteSyntaxClass(AstNodeFamily family)
        {
            using (WriteBlock("public static class Syntax"))
            {
                Delimit(family.Concepts, "syntaxMethod", Environment.NewLine);
            }
        }

        protected String Visit(AstNodeContract contract, String mode)
        {
            var baseTypes = contract.BaseTypes.ToArray();

            if (mode == "public")
            {
                var publicBaseTypes = baseTypes.Or(new[] { "IVisitableChildren" }).Delimit(", ");

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

        private IEnumerable<T> GetConceptHierarchy<T>(Maybe<T> concept)
            where T : IAstConcept
        {
            return concept.Recurse(x => x.BaseTypes.SelectMaybe(y => Resolve(x, y)).OfType<T>());
        }

        private IEnumerable<IAstConcept> GetParentHierarchy<T>(IEnumerable<T> conceptHierarchy)
            where T : IAstConcept
        {
            return conceptHierarchy.SelectMaybe(x => x.ParentType.SelectMaybe(y => Resolve(x, y)));
        }

        protected String Visit(AstNode node, String mode)
        {
            var conceptHierarchy = GetConceptHierarchy(node.ToMaybe<IAstConcept>()).ToArray();
            var nodeHierarcy = conceptHierarchy.OfType<AstNode>().ToArray();

            var parentHierarcy = GetParentHierarchy(nodeHierarcy).ToArray();
            bool parentDefinedByNode = parentHierarcy.Any();

            if (!parentDefinedByNode)
                parentHierarcy = GetParentHierarchy(conceptHierarchy).ToArray();

            var baseNode = nodeHierarcy.Skip(1).TryFirst();
            var parentNode = parentHierarcy.TryFirst();
            var closestParentNode = GetParentHierarchy(nodeHierarcy.Skip(1)).TryFirst();

            var farthestBaseNode = nodeHierarcy.Last();
            var farthestParentNode = parentHierarcy.TryLast();

            var baseTypes = node.BaseTypes.Concat(new[] { String.Format("IAstNode<Internal.{0}>", node.TypeName) });

            if (mode == "public")
            {
                String baseTypesSuffix = String.Format(" : {0}", baseTypes.Delimit(", "));

                using (WriteBlock("public {0}{1} class {2}{3}",
                                  node.IsAbstract ? "abstract " : "",
                                  node.IsPartial ? "partial " : "",
                                  node.TypeName,
                                  baseTypesSuffix))
                {
                    bool generatePrivateField = parentNode.HasValue &&
                                                ((closestParentNode.HasValue && parentNode != closestParentNode) ||
                                                 (!closestParentNode.HasValue));
                    if (generatePrivateField)
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
                        if (generatePrivateField)
                            WriteLine("_parent = parent;");

                        WriteLine("_underlying = underlying;");
                    }

                    WriteLine();

                    if (parentNode.HasValue)
                    {
                        if (closestParentNode.HasValue && parentNode != closestParentNode)
                            WriteLine("public new {0} Parent {{ get {{ return _parent; }} }}", parentNode.Value.TypeName);
                        else if (!closestParentNode.HasValue)
                            WriteLine("public {0} Parent {{ get {{ return _parent; }} }}", parentNode.Value.TypeName);
                    }

                    if (!baseNode.HasValue)
                    {
                        var contractsWithParent = conceptHierarchy
                            .OfType<AstNodeContract>()
                            .Distinct()
                            .Where(x => x.ParentType.HasValue);


                        foreach (var contractWithParent in contractsWithParent)
                            WriteLine("{0} {1}.Parent {{ get {{ return Parent; }} }}", contractWithParent.ParentType.Value, contractWithParent.TypeName);
                    }

                    WriteLine("Internal.{0} IAstNode<Internal.{0}>.GetUnderlying() {{ return _underlying; }}", node.TypeName);
                    WriteLine();

                    String acceptChildrenModifier = baseNode.HasValue ? "override" : "virtual";

                    Boolean hasNodeProperties = node.Properties.SelectMaybe(x => Resolve(node, x.Type)).Any();

                    if (!baseNode.HasValue || hasNodeProperties)
                    {
                        using (WriteBlock("public {0} void AcceptChildren(Action<IEnumerable<Object>> dispatch)", acceptChildrenModifier))
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
                    .Concat(new[] { String.Format("IAstUnderlyingNode<SyntacticModel.{0}, {1}>", node.TypeName, parentType) });

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
                            DispatchChildren(node, "assignToField");
                        }
                        WriteLine();
                    }

                    String makePublicInheritanceModifier = !baseNode.HasValue || closestParentNode != parentNode ? "" : "new ";
                    using (WriteBlock("public {0}SyntacticModel.{1} MakePublic({2} parent)", makePublicInheritanceModifier, node.TypeName, parentType))
                    {
                        WriteLine(farthestBaseNode == node
                                    ? "return BuildPublic(parent);"
                                    : "return (SyntacticModel.{0}) BuildPublic(parent);",
                                  node.TypeName);
                    }
                    WriteLine();

                    String buildParentParameter = farthestParentNode.HasValue
                        ? String.Format("SyntacticModel.{0} parent", farthestParentNode.Value.TypeName)
                        : "Object parent";

                    String buildParentArgument = "";

                    if (parentNode.HasValue)
                    {
                        buildParentArgument = parentNode.Value != farthestParentNode.Value
                            ? String.Format("(SyntacticModel.{0}) parent, ", parentNode.Value.TypeName)
                            : "parent, ";
                    }

                    if (node.IsAbstract && farthestBaseNode == node)
                    {
                        WriteLine("protected abstract SyntacticModel.{0} BuildPublic({1});", farthestBaseNode.TypeName, buildParentParameter);
                    }
                    else if (!node.IsAbstract)
                    {
                        String buildPublicInheritanceModifier = farthestBaseNode == node ? "virtual" : "override";
                        using (WriteBlock("protected {0} SyntacticModel.{1} BuildPublic({2})", buildPublicInheritanceModifier, farthestBaseNode.TypeName, buildParentParameter))
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
                Write("public static {0} {1}(", node.TypeName, node.SimpleName);
                DispatchChildren(nodeHierarcy, "parameter", (s, l, r) => { Write(", "); return s; });
                using (WriteBlock(")"))
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
                          GetPrivatePropertyType(property),
                          Camelize(property.SimpleName));
            }

            if (mode == "parameter")
            {
                Write("{0} {1}",
                      GetPublicPropertyType(property),
                      SafeIdentifier(Camelize(property.SimpleName)));
            }

            if (mode == "argument")
            {
                Write("{0}", SafeIdentifier(Camelize(property.SimpleName)));
            }

            if (mode == "assignToField")
            {
                String format = property.Cardinality == AstNodePropertyCardinality.Many
                    ? "_{0} = {1}.ToArray();"
                    : "_{0} = {1};";

                WriteLine(format, Camelize(property.SimpleName), SafeIdentifier(Camelize(property.SimpleName)));
            }

            if (mode == "property")
            {
                WriteLine("public {0} {1} {{ get {{ return _{2}; }} }}",
                          GetPublicPropertyType(property),
                          property.SimpleName,
                          Camelize(property.SimpleName));
            }

            if (mode == "contractProperty")
            {
                WriteLine("{0} {1} {{ get; }}",
                          GetPublicPropertyType(property),
                          property.SimpleName);
            }

            if (mode == "publicSelector")
            {
                using (WriteBlock("public {0} {1}", GetPublicPropertyType(property), property.SimpleName))
                using (WriteBlock("get"))
                {
                    if (isNodeProperty)
                    {
                        WriteLine(property.Cardinality != AstNodePropertyCardinality.One
                                    ? "return ((IAstNode<Internal.{0}>)this).GetUnderlying().{1}.Select(x => ((IAstUnderlyingNode<{2}, {0}>)x).MakePublic(this));"
                                    : "return ((IAstUnderlyingNode<{2}, {0}>)((IAstNode<Internal.{0}>)this).GetUnderlying().{1}).MakePublic(this);",
                                  property.Parent.TypeName, property.SimpleName, property.Type);
                    }
                    else
                    {
                        WriteLine("return ((IAstNode<Internal.{0}>)this).GetUnderlying().{1};", property.Parent.TypeName, property.SimpleName);
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
                          SafeIdentifier(Camelize(property.SimpleName)),
                          property.Type);
                }
                else
                    Write(SafeIdentifier(Camelize(property.SimpleName)));
            }

            if (mode == "dispatchInvoke" && isNodeProperty)
            {
                if (property.Cardinality == AstNodePropertyCardinality.One)
                    WriteLine("dispatch(new[]{{ {0} }});", property.SimpleName);

                if (property.Cardinality == AstNodePropertyCardinality.ZeroOrOne)
                    WriteLine("dispatch({0}.ToEnumerable());", property.SimpleName);

                if (property.Cardinality == AstNodePropertyCardinality.Many)
                    WriteLine("dispatch({0});", property.SimpleName);
            }

            return mode;
        }

        private String GetPrivatePropertyType(AstNodeProperty property)
        {
            if (property.Cardinality == AstNodePropertyCardinality.One)
                return property.Type;

            if (property.Cardinality == AstNodePropertyCardinality.ZeroOrOne)
                return String.Format("Maybe<{0}>", property.Type);

            if (property.Cardinality == AstNodePropertyCardinality.Many)
                return String.Format("{0}[]", property.Type);

            throw new InvalidOperationException("Unrecognized cardinality.");
        }


        private String GetPublicPropertyType(AstNodeProperty property)
        {
            if (property.Cardinality == AstNodePropertyCardinality.One)
                return property.Type;

            if (property.Cardinality == AstNodePropertyCardinality.ZeroOrOne)
                return String.Format("Maybe<{0}>", property.Type);

            if (property.Cardinality == AstNodePropertyCardinality.Many)
                return String.Format("IEnumerable<{0}>", property.Type);

            throw new InvalidOperationException("Unrecognized cardinality.");
        }
    }
}
