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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using iSynaptic.Commons;

namespace iSynaptic
{
    public class AggregateMemento<TIdentifier> : AggregateMemento
        where TIdentifier : IEquatable<TIdentifier>
    {
        public static readonly AggregateMemento<TIdentifier> Empty 
            = new AggregateMemento<TIdentifier>();

        private AggregateMemento()
        {
        }

        public AggregateMemento(Type aggregateType, Maybe<IAggregateSnapshot<TIdentifier>> snapshot, IEnumerable<IAggregateEvent<TIdentifier>> events)
            : base(aggregateType, snapshot.Cast<Object>(), events)
        {
        }

        public new Maybe<IAggregateSnapshot<TIdentifier>> Snapshot { get { return base.Snapshot.Cast<IAggregateSnapshot<TIdentifier>>(); } }
        public new IEnumerable<IAggregateEvent<TIdentifier>> Events { get { return (IEnumerable<IAggregateEvent<TIdentifier>>)base.Events; } }
    }

    public class AggregateMemento
    {
        internal AggregateMemento()
        {
        }

        internal AggregateMemento(Type aggregateType, IMaybe<Object> snapshot, IEnumerable events)
        {
            Snapshot = snapshot;
            Events = events;
            AggregateType = Guard.NotNull(aggregateType, "aggregateType");
        }

        public Type AggregateType { get; private set; }
        protected IMaybe<Object> Snapshot { get; set; }
        protected IEnumerable Events { get; set; }

        internal AggregateMemento<TIdentifier> ToMemento<TIdentifier>()
            where TIdentifier : IEquatable<TIdentifier>
        {
            return new AggregateMemento<TIdentifier>(AggregateType, 
                Snapshot.Cast<IAggregateSnapshot<TIdentifier>>(),
                Events.Cast<IAggregateEvent<TIdentifier>>()
            );
        }
    }
}