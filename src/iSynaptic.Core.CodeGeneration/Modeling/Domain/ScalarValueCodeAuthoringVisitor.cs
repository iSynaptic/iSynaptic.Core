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
using System.Linq;
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;
using iSynaptic.Commons.Text;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public class ScalarValueCodeAuthoringVisitor : DomainCodeAuthoringVisitor<string>
    {
        public ScalarValueCodeAuthoringVisitor(IndentingTextWriter writer, SymbolTable symbolTable)
            : base(writer, symbolTable)
        {
        }

        protected BuiltInType GetBuiltInType(ScalarValueSyntax value)
        {
            var scalarValue =
                value.RecurseSelect(x => SymbolTable.Resolve(x, x.Base).Symbol.ToMaybe<ScalarValueSyntax>(),
                                    x => SymbolTable.Resolve(x, x.Base).Symbol)
                     .Last();

            return (BuiltInType)scalarValue;
        }

        protected void Visit(ScalarValueSyntax value)
        {
            if (value.IsExternal)
                return;

            var builtInType = GetBuiltInType(value);
            var baseType = (IType)SymbolTable.Resolve(value, value.Base).Symbol;
            var isBaseScalarValue = baseType is ScalarValueSyntax;

            var baseTypes = new List<String>();
            baseTypes.AddRange(isBaseScalarValue
                ? new []{value.Base.ToString()}
                : new []{String.Format("IScalarValue<{0}>", value.Base), String.Format("IEquatable<{0}>", value.Base)});

            baseTypes.Add(String.Format("IEquatable<{0}>", value.Name));

            WriteLine("[ValueObject]");
            using (WriteBlock("public {0}{1}class {2}{3}",
                              value.IsAbstract ? "abstract " : "",
                              value.IsPartial ? "partial " : "",
                              value.Name,
                              String.Format(" : {0}", baseTypes.Delimit(", "))))
            {
                if (!isBaseScalarValue)
                    WriteLine("private readonly {0} _value;", value.Base);

                WriteGeneratedCodeAttribute();
                WriteLine();
                using (WriteBlock("public {0}({1} value){2}", 
                                  value.Name,
                                  builtInType.FullName,
                                  isBaseScalarValue ? " : base(value)" : ""))
                {
                    if(value.IsPartial)
                        WriteLine("Validate(value);");

                    if (!isBaseScalarValue)
                        WriteLine("_value = value;");
                }

                if (value.IsPartial)
                {
                    WriteLine();
                    WriteLine("partial void Validate({0} value);", builtInType.FullName);
                }

                WriteLine();
                WriteGeneratedCodeAttribute();
                WriteLine();
                using (WriteBlock("public Boolean Equals({0} other)", value.Name))
                {
                    WriteLine("return Equals((Object)other);");
                }
                WriteLine();

                if (isBaseScalarValue)
                    return;

                WriteGeneratedCodeAttribute();
                WriteLine();
                using (WriteBlock("public Boolean Equals({0} other)", value.Base))
                {
                    WriteLine("return Value == other;");
                }
                WriteLine();

                WriteGeneratedCodeAttribute();
                WriteLine();
                using (WriteBlock("public override Boolean Equals(Object obj)"))
                {
                    WriteLine("{0} other = obj as {0};", value.Name);
                    WriteLine();

                    WriteLine("if (ReferenceEquals(other, null)) return false;");
                    WriteLine("if (GetType() != other.GetType()) return false;");
                    WriteLine();

                    WriteLine("if(Value != other.Value) return false;");
                    WriteLine("return true;");
                }

                WriteLine();

                WriteGeneratedCodeAttribute();
                WriteLine();
                using (WriteBlock("public override Int32 GetHashCode()"))
                {
                    WriteLine("return Value.GetHashCode();");
                }

                WriteLine();

                WriteGeneratedCodeAttribute();
                WriteLine(" public static implicit operator {0}({1} value) {{ return new {0}(value); }}", value.Name, value.Base);

                WriteGeneratedCodeAttribute();
                WriteLine(" public static implicit operator {0}({1} value) {{ return value.Value; }}", value.Base, value.Name);

                WriteLine();
                WriteGeneratedCodeAttribute();
                WriteLine();
                using (WriteBlock("public static Boolean operator ==({0} left, {0} right)", value.Name))
                {
                    WriteLine("if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;");
                    WriteLine("return ReferenceEquals(left, null) || left.Equals(right);");
                }
                WriteLine();

                WriteGeneratedCodeAttribute();
                WriteLine();
                WriteLine("public static Boolean operator !=({0} left, {0} right) {{ return !(left == right); }}", value.Name);

                WriteLine();
                WriteGeneratedCodeAttribute();
                WriteLine();
                WriteLine("public {0} Value {{ get {{ return _value; }} }}", builtInType.FullName);

                WriteLine();
                WriteGeneratedCodeAttribute();
                WriteLine();
                WriteLine("object IScalarValue.Value { get { return Value; } }");
            }
        }
    }
}