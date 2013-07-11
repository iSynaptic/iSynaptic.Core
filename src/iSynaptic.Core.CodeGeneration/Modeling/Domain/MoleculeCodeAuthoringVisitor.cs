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
    public abstract class MoleculeCodeAuthoringVisitor : DomainCodeAuthoringVisitor<String>
    {
        protected MoleculeCodeAuthoringVisitor(IndentingTextWriter writer, SymbolTable symbolTable)
            : base(writer, symbolTable)
        {
        }

        protected virtual bool ShouldBeEquatable(MoleculeSyntax molecule)
        {
            return true;
        }

        protected virtual Maybe<String> GetBaseMolecule(MoleculeSyntax molecule)
        {
            return molecule.Base.Select(x => x.ToString());
        }

        protected virtual IEnumerable<String> GetInheritedTypes(MoleculeSyntax molecule)
        {
            var baseMolecule = GetBaseMolecule(molecule);
            if (baseMolecule.HasValue)
                yield return baseMolecule.Value;

            if (ShouldBeEquatable(molecule))
                yield return String.Format("IEquatable<{0}>", molecule.Name);
        }

        protected virtual void Visit(MoleculeSyntax molecule)
        {
            if (molecule.IsExternal)
                return;

            var baseValue = molecule.Base
                .Select(x => SymbolTable.Resolve(molecule.Parent, x).Symbol)
                .Cast<MoleculeSyntax>();

            var baseAtoms = GetBaseAtomInfo(molecule, baseValue).ToArray();

            String baseTypes = GetInheritedTypes(molecule).Delimit(", ");
            if (baseTypes.Length > 0)
                baseTypes = String.Format(" : {0}", baseTypes);

            if(molecule is ValueSyntax)
                WriteLine("[ValueObject]");

            using (WriteBlock("public {0}{1}class {2}{3}", 
                              molecule.IsAbstract ? "abstract " : "",
                              molecule.IsPartial ? "partial " : "",
                              molecule.Name,
                              baseTypes))
            {
                Dispatch(molecule.Atoms, "field");
                WriteLine();

                Write("{0} {1}(", molecule.IsAbstract ? "protected" : "public", molecule.Name);
                Delimit(molecule.Atoms.Select(GetAtomInfo).Concat(baseAtoms), "parameter", ", ");
                WriteLine(")");

                if (baseAtoms.Length > 0)
                {
                    Write("    : base(");
                    Delimit(baseAtoms, "argument", ", ");
                    WriteLine(")");
                }

                using (WithBlock())
                {
                    Dispatch(molecule.Atoms, "validateArgument");

                    if (molecule.IsPartial)
                    {
                        Write("Validate(");
                        Delimit(molecule.Atoms, "argument", ", ");
                        WriteLine(");");
                    }

                    WriteLine();
                    Dispatch(molecule.Atoms, "assignToField");
                }
                WriteLine();

                if (molecule.IsPartial)
                {
                    Write("partial void Validate(");
                    Delimit(molecule.Atoms, "parameter", ", ");
                    WriteLine(");");
                    WriteLine();
                }

                if (ShouldBeEquatable(molecule))
                {
                    WriteEquatableImplementation(molecule, baseValue.HasValue);
                    WriteLine();
                }

                Dispatch(molecule.Atoms, "property");
            }
        }

        protected virtual IEnumerable<AtomInfo> GetBaseAtomInfo(MoleculeSyntax molecule, Maybe<MoleculeSyntax> baseValue)
        {
            return baseValue
                .Recurse(x => x.Base.Select(b => SymbolTable.Resolve(molecule, b).Symbol).Cast<MoleculeSyntax>())
                .SelectMany(x => x.Atoms)
                .Select(GetAtomInfo);
        }

        private void WriteEquatableImplementation(MoleculeSyntax molecule, Boolean hasBase)
        {
            using (WriteBlock("public Boolean Equals({0} other)", molecule.Name))
            {
                WriteLine("return Equals((Object)other);");
            }
            WriteLine();

            if (!molecule.Atoms.Any() && hasBase) 
                return;

            using (WriteBlock("public override Boolean Equals(Object obj)"))
            {
                WriteLine("{0} other = obj as {0};", molecule.Name);
                WriteLine();

                WriteLine("if (ReferenceEquals(other, null)) return false;");
                WriteLine("if (GetType() != other.GetType()) return false;");
                WriteLine();

                Dispatch(molecule.Atoms, "equalsCheck");
                WriteLine();

                WriteLine(hasBase ? "return base.Equals(obj);" : "return true;");
            }

            WriteLine();

            using (WriteBlock("public override Int32 GetHashCode()"))
            {
                WriteLine(hasBase ? "int hash = base.GetHashCode();" : "int hash = 1;");
                WriteLine();

                Dispatch(molecule.Atoms, "mixInHash");

                WriteLine();
                WriteLine("return hash;");
            }

            WriteLine();

            using (WriteBlock("public static Boolean operator ==({0} left, {0} right)", molecule.Name))
            {
                WriteLine("if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;");
                WriteLine("return ReferenceEquals(left, null) || left.Equals(right);");
            }
            WriteLine();

            WriteLine("public static Boolean operator !=({0} left, {0} right) {{ return !(left == right); }}", molecule.Name);
        }

        protected class AtomInfo
        {
            private readonly SimpleNameSyntax _name;
            private readonly TypeReferenceSyntax _type;
            private readonly Boolean _isValueType;

            public AtomInfo(SimpleNameSyntax name, TypeReferenceSyntax type, Boolean isValueType)
            {
                _name = name;
                _type = type;
                _isValueType = isValueType;
            }

            public SimpleNameSyntax Name { get { return _name; } }
            public TypeReferenceSyntax Type { get { return _type; } }
            public Boolean IsValueType { get { return _isValueType; } }
        }

        protected virtual AtomInfo GetAtomInfo(AtomSyntax atom)
        {
            var typeSymbol = SymbolTable.Resolve(atom.Parent.Parent, atom.Type.Name).Symbol;
            var type = typeSymbol as IType;

            if (type == null)
                throw new InvalidOperationException("Unable to find type of property.");

            if (!type.HasValueSemantics)
                throw new InvalidOperationException("Only types with value semantics can be used in molecule atoms.");

            return new AtomInfo(atom.SimpleName, atom.Type, type.IsValueType);
        }

        protected virtual void Visit(AtomSyntax atom, String mode)
        {
            this.Dispatch(GetAtomInfo(atom), mode);
        }

        protected virtual void Visit(AtomInfo atom, String mode)
        {
            var cardinality = atom.Type.Cardinality;

            String typeString = GetTypeString(atom.Type);
            String fieldName = String.Format("_{0}", Camelize(atom.Name));
            String argumentName = SafeIdentifier(Camelize(atom.Name));
            String propertyName = Pascalize(atom.Name);

            if (mode == "field")
                WriteLine("private readonly {0} {1};", typeString, fieldName);

            if(mode == "property")
                WriteLine("public {0} {1} {{ get {{ return {2}; }} }}", typeString, propertyName, fieldName);

            if (mode == "validateArgument")
            {
                if(cardinality.IsMany|| (!cardinality.IsOptional && !atom.IsValueType))
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
