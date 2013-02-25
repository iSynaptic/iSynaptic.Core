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
using Sprache;

namespace iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel
{
    public class BuiltInType : IType
    {
        public static readonly IEnumerable<BuiltInType> Types = new[]
        {
            new BuiltInType(typeof(Int16)),
            new BuiltInType(typeof(Int32)),
            new BuiltInType(typeof(Int64)),
            new BuiltInType(typeof(UInt16)),
            new BuiltInType(typeof(UInt32)),
            new BuiltInType(typeof(UInt64)),
            new BuiltInType(typeof(Single)),
            new BuiltInType(typeof(Double)),
            new BuiltInType(typeof(Decimal)),
            new BuiltInType(typeof(DateTime)),
            new BuiltInType(typeof(Guid)),
            new BuiltInType(typeof(String))
        };

        private readonly Type _actualType;
        private readonly NameSyntax _fullName;

        private BuiltInType(Type actualType)
        {
            _actualType = actualType;
            _fullName = Parser.Name.Parse(actualType.FullName);
        }

        public void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch) { }

        public INode Parent { get { return null; } }

        public Boolean IsValueType { get { return ActualType.IsValueType; } }
        public Boolean HasValueSemantics { get { return true; } }

        public Type ActualType { get { return _actualType; } }
        public NameSyntax FullName { get { return _fullName; } }

        public NameSyntax Name { get { return SimpleName; } }
        public SimpleNameSyntax SimpleName { get { return FullName.SimpleName; } }
    }
}
