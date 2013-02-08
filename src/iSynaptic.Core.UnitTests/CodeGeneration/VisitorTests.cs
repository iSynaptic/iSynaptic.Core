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
using NUnit.Framework;

namespace iSynaptic.CodeGeneration
{
    [TestFixture]
    public class VisitorTests
    {
        private class TestSubject : IVisitable
        {
            public TestSubject(Int32 value)
                : this(value, null)
            {
            }

            public TestSubject(Int32 value, params TestSubject[] children)
            {
                Value = value;
                Children = children ?? Enumerable.Empty<TestSubject>();
            }

            public TState AcceptChildren<TState>(IVisitor visitor, TState state)
            {
                return visitor.Dispatch(Children, state);
            }

            public Int32 Value { get; private set; }
            private IEnumerable<TestSubject> Children { get; set; }
        }

        private class StatefulTestVisitor : Visitor<Int32>
        {
            protected Int32 Visit(TestSubject subject, Int32 value)
            {
                return subject.AcceptChildren(this, value + subject.Value);
            }
        }

        private class StatelessTestVisitor : Visitor
        {
            public Int32 Result { get; private set; }

            protected void Visit(TestSubject subject)
            {
                Result += subject.Value;
                subject.AcceptChildren<Object>(this, null);
            }
        }

        [Test]
        public void Visitor_ThreadsStateCorrectly()
        {
            var subject = new TestSubject(1,
                    new TestSubject(2),
                    new TestSubject(3,
                        new TestSubject(4),
                        new TestSubject(5)));

            var visitor = new StatefulTestVisitor();
            var result = visitor.Dispatch(subject, 0);

            Assert.AreEqual(15, result);
        }

        [Test]
        public void Visitor_WhenNotThreadingState_WorksCorrectly()
        {
            var subject = new TestSubject(1,
                    new TestSubject(2),
                    new TestSubject(3,
                        new TestSubject(4),
                        new TestSubject(5)));

            var visitor = new StatelessTestVisitor();
            visitor.Dispatch(subject);

            Assert.AreEqual(15, visitor.Result);
        }
    }
}
