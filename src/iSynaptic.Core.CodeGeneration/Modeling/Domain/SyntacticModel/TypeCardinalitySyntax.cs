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
    public class TypeCardinalitySyntax : IEquatable<TypeCardinalitySyntax>
    {
        private readonly Int32 _minimum;
        private readonly Maybe<Int32> _maximum;

        public TypeCardinalitySyntax(Int32 minimum)
        {
            _minimum = minimum;
        }

        public TypeCardinalitySyntax(Int32 minimum, Int32 maximum)
        {
            _minimum = minimum;
            _maximum = maximum.ToMaybe();
        }
        
        public Boolean Equals(TypeCardinalitySyntax other)
        {
            return !ReferenceEquals(other, null) &&
                   Minimum == other.Minimum &&
                   Maximum == other.Maximum;
        }

        public override Boolean Equals(object obj)
        {
            return Equals(obj as TypeCardinalitySyntax);
        }

        public override Int32 GetHashCode()
        {
            Int32 hash = 1;
            hash = HashCode.MixJenkins32(hash + Minimum);
            hash = HashCode.MixJenkins32(hash + (Maximum.HasValue ? Maximum.Value : 0));
            return hash;
        }

        public static Boolean operator ==(TypeCardinalitySyntax left, TypeCardinalitySyntax right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        public static Boolean operator !=(TypeCardinalitySyntax left, TypeCardinalitySyntax right)
        {
            return !(left != right);
        }

        public Int32 Minimum { get { return _minimum; } }
        public Maybe<Int32> Maximum { get { return _maximum; } }

        public Boolean IsOptional { get { return Minimum <= 0; } }
        public Boolean IsMany { get { return !Maximum.HasValue || Maximum.Value > 1; } }
    }
}
