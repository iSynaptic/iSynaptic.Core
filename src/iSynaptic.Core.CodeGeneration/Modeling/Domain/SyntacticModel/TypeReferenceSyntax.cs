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
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel
{
    public class TypeReferenceSyntax : IEquatable<TypeReferenceSyntax>
    {
        private readonly NameSyntax _name;
        private readonly TypeCardinalitySyntax _cardinality;

        public TypeReferenceSyntax(NameSyntax name, TypeCardinalitySyntax cardinality)
        {
            _name = Guard.NotNull(name, "name");
            _cardinality = Guard.NotNull(cardinality, "cardinality");
        }


        public Boolean Equals(TypeReferenceSyntax other)
        {
            return !ReferenceEquals(other, null) &&
                   Name == other.Name &&
                   Cardinality == other.Cardinality;
        }

        public override Boolean Equals(object obj)
        {
            return Equals(obj as TypeReferenceSyntax);
        }

        public override Int32 GetHashCode()
        {
            Int32 hash = 1;
            hash = HashCode.MixJenkins32(hash + Name.GetHashCode());
            hash = HashCode.MixJenkins32(hash + Cardinality.GetHashCode());
            return hash;
        }

        public static Boolean operator ==(TypeReferenceSyntax left, TypeReferenceSyntax right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        public static Boolean operator !=(TypeReferenceSyntax left, TypeReferenceSyntax right)
        {
            return !(left != right);
        }

        public NameSyntax Name { get { return _name; } }
        public TypeCardinalitySyntax Cardinality { get { return _cardinality; } }
    }
}