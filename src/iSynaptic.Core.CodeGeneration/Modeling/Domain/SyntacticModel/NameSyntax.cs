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
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;

namespace iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel
{
    public abstract partial class NameSyntax : IEquatable<NameSyntax>
    {
        public static NameSyntax operator+(NameSyntax left, NameSyntax right)
        {
            Guard.NotNull(left, "left");
            Guard.NotNull(right, "right");

            var simpleName = right as SimpleNameSyntax;
            if (simpleName != null)
                return Syntax.QualifiedName(left, simpleName);

            var qns = (QualifiedNameSyntax) right;
            return qns.Parts.Aggregate(left, Syntax.QualifiedName);
        }

        public bool StartsWith(NameSyntax other)
        {
            Guard.NotNull(other, "other");
            return ToString().StartsWith(other.ToString());
        }

        public bool Equals(NameSyntax other)
        {
            return !ReferenceEquals(other, null) &&
                   ToString() == other.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NameSyntax);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator==(NameSyntax left, NameSyntax right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        public static bool operator !=(NameSyntax left, NameSyntax right)
        {
            return !(left == right);
        }

        public abstract SimpleNameSyntax SimpleName { get; }
        public abstract IEnumerable<SimpleNameSyntax> Parts { get; }
    }
}