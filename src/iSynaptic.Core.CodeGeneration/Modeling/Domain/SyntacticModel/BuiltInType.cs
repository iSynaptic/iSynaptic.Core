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
using Sprache;

namespace iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel
{
    public class BuiltInType : IType
    {
        public static readonly IEnumerable<BuiltInType> Types = new[]
        {
            new BuiltInType((NameSyntax)"void", typeof(void)),
            new BuiltInType((NameSyntax)"bool", typeof(bool)),
            new BuiltInType((NameSyntax)"byte", typeof(byte)),
            new BuiltInType((NameSyntax)"char", typeof(char)),
            new BuiltInType((NameSyntax)"decimal", typeof(decimal)),
            new BuiltInType((NameSyntax)"double", typeof(double)),
            new BuiltInType((NameSyntax)"float", typeof(float)),
            new BuiltInType((NameSyntax)"short", typeof(short)),
            new BuiltInType((NameSyntax)"int", typeof(int)),
            new BuiltInType((NameSyntax)"long", typeof(long)),
            new BuiltInType((NameSyntax)"uint", typeof(uint)),
            new BuiltInType((NameSyntax)"ulong", typeof(ulong)),
            new BuiltInType((NameSyntax)"ushort", typeof(ushort)),
            new BuiltInType((NameSyntax)"sbyte", typeof(sbyte)),
            new BuiltInType((NameSyntax)"string", typeof(string)),
            new BuiltInType((NameSyntax)"guid", typeof(Guid)),
            new BuiltInType((NameSyntax)"datetime", typeof(DateTime)),
            new BuiltInType((NameSyntax)"timespan", typeof(TimeSpan))
        };

        private readonly NameSyntax _alias;
        private readonly Type _actualType;
        private readonly NameSyntax _fullName;

        private BuiltInType(NameSyntax alias, Type actualType)
        {
            _alias = alias;
            _actualType = actualType;
            _fullName = Parser.Name.Parse(actualType.FullName);
        }

        public void AcceptChildren(Action<IEnumerable<Object>> dispatch) { }

        public INode Parent { get { return null; } }

        public Boolean IsExternal { get { return true; } }
        public Boolean IsValueType { get { return ActualType.IsValueType; } }
        public Boolean HasValueSemantics { get { return true; } }

        public Type ActualType { get { return _actualType; } }
        public NameSyntax FullName { get { return _fullName; } }

        public NameSyntax Alias { get { return _alias; } }
        public NameSyntax Name { get { return SimpleName; } }
        public SimpleNameSyntax SimpleName { get { return FullName.SimpleName; } }
    }
}
