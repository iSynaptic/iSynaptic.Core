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

namespace iSynaptic.Modeling.Domain
{
    public abstract class AggregateLoadFrame
    {
        protected AggregateLoadFrame(Type aggregateType, object id)
        {
            AggregateType = Guard.NotNull(aggregateType, "aggregateType");
            Id = id;
        }

        public Type AggregateType { get; private set; }
        public object Id { get; private set; }
    }

    public abstract class AggregateLoadFrame<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        protected AggregateLoadFrame(Type aggregateType, TIdentifier id)
        {
            AggregateType = Guard.NotNull(aggregateType, "aggregateType");
            Id = id;
        }

        public Type AggregateType { get; private set; }
        public TIdentifier Id { get; private set; }
    }
}