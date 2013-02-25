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
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;
using iSynaptic.Commons.Text;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public class ValueCodeAuthoringVisitor : DomainCodeAuthoringVisitor<String>
    {
        public ValueCodeAuthoringVisitor(IndentingTextWriter writer, SymbolTable symbolTable)
            : base(writer, symbolTable)
        {
        }

        protected void Visit(ValueSyntax value)
        {
            var baseValue = value.Base
                .Select(x => SymbolTable.Resolve(value.Parent, x).Symbol)
                .Cast<ValueSyntax>();

            var baseProperties = baseValue
                .Recurse(x => x.Base.Select(b => SymbolTable.Resolve(value, b).Symbol).Cast<ValueSyntax>())
                .SelectMany(x => x.Properties)
                .ToArray();

            using (WriteBlock("public {0}class {1} : {2}IEquatable<{1}>", 
                              value.IsAbstract ? "abstract " : "", 
                              value.Name, 
                              value.Base.Select(x => x + ", ").ValueOrDefault("")))
            {
                Dispatch(value.Properties, "field");
                WriteLine();

                Write("{0} {1}(", value.IsAbstract ? "protected" : "public", value.Name);
                Delimit(value.Properties.Concat(baseProperties), "parameter", ", ");
                WriteLine(")");

                if (baseValue.HasValue)
                {
                    Write("    : base(");
                    Delimit(baseProperties, "argument", ", ");
                    WriteLine(")");
                }

                using (WithBlock())
                {
                    Dispatch(value.Properties, "validateArgument");
                    WriteLine();
                    Dispatch(value.Properties, "assignToField");
                }
                WriteLine();

                WriteEquatableImplementation(value, baseValue.HasValue);
                WriteLine();

                Dispatch(value.Properties, "property");
            }
        }

        private void WriteEquatableImplementation(ValueSyntax value, Boolean hasBase)
        {
            using (WriteBlock("public Boolean Equals({0} other)", value.Name))
            {
                WriteLine("return Equals((Object)other);");
            }
            WriteLine();

            if (value.Properties.Any())
            {
                using (WriteBlock("public override Boolean Equals(Object obj)"))
                {
                    WriteLine("{0} other = obj as {0};", value.Name);
                    WriteLine();

                    WriteLine("if (ReferenceEquals(other, null)) return false;");
                    WriteLine("if (GetType() != other.GetType()) return false;");
                    WriteLine();

                    Dispatch(value.Properties, "equalsCheck");
                    WriteLine();

                    WriteLine(hasBase ? "return base.Equals(obj);" : "return true;");
                }
                WriteLine();

                using (WriteBlock("public override Int32 GetHashCode()"))
                {
                    WriteLine(hasBase ? "int hash = base.GetHashCode();" : "int hash = 1;");
                    WriteLine();

                    Dispatch(value.Properties, "mixInHash");

                    WriteLine();
                    WriteLine("return hash;");
                }

                WriteLine();
            }

            if (!hasBase)
            {
                using (WriteBlock("public static bool operator ==({0} left, {0} right)", value.Name))
                {
                    WriteLine("if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;");
                    WriteLine("return ReferenceEquals(left, null) || left.Equals(right);");
                }
                WriteLine();

                WriteLine("public static bool operator !=({0} left, {0} right) {{ return !(left == right); }}", value.Name);
            }
        }

        protected void Visit(ValuePropertySyntax property, String mode)
        {
            var cardinality = property.Type.Cardinality;

            var typeSymbol = SymbolTable.Resolve(property, property.Type.Name).Symbol;
            var type = typeSymbol as IType;

            if(type == null)
                throw new InvalidOperationException("Unable to find type of property.");

            if(!(type is BuiltInType || type is ValueSyntax))
                throw new InvalidOperationException("Only built-in or value types can be used in value properties.");

            String typeString = GetTypeString(property.Type, property);
            String fieldName = String.Format("_{0}", Camelize(property.Name));
            String argumentName = SafeIdentifier(Camelize(property.Name));
            String propertyName = Pascalize(property.Name);

            if (mode == "field")
                WriteLine("private readonly {0} {1};", typeString, fieldName);

            if(mode == "property")
                WriteLine("public {0} {1} {{ get {{ return {2}; }} }}", typeString, propertyName, fieldName);

            if (mode == "validateArgument")
            {
                if(cardinality.IsMany|| (!cardinality.IsOptional && !type.IsValueType))
                    WriteLine("if(ReferenceEquals({0}, null)) throw new ArgumentNullException(\"{0}\");", argumentName);
            }

            if(mode == "parameter")
                Write("{0} {1}", typeString, argumentName);

            if(mode == "argument")
                Write(argumentName);

            if (mode == "assignToField")
                WriteLine("{0} = {1};", fieldName, argumentName);

            if (mode == "equalsCheck")
            {
                if (!cardinality.IsMany)
                {
                    String prefix = "";
                    if (cardinality.IsOptional)
                    {
                        prefix = String.Format("{0}.HasValue && ", propertyName);
                        WriteLine("if ({0}.HasValue != other.{0}.HasValue) return false;", propertyName);
                    }

                    WriteLine("if({0}!{1}.Equals(other.{1})) return false;", prefix, propertyName);
                }
                else
                    WriteLine("if (!{0}.SequenceEqual(other.{0})) return false;", propertyName);
            }

            if (mode == "mixInHash")
            {
                if (!cardinality.IsMany)
                    WriteLine("hash = HashCode.MixJenkins32(hash + {0}.GetHashCode());", propertyName);
                else
                    WriteLine("hash = {0}.Aggregate(hash, (h, item) => HashCode.MixJenkins32(h + item.GetHashCode()));", propertyName);
            }
        }
    }
}
