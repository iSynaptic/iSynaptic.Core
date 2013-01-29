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

namespace iSynaptic
{
    public class AggregateEventStream<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly List<AggregateEvent<TIdentifier>> _events
            = new List<AggregateEvent<TIdentifier>>();

        private Int32 _startVersion;
        private Int32 _uncommittedOffset;

        public void AppendEvent(AggregateEvent<TIdentifier> @event)
        {
            Guard.NotNull(@event, "event");

            lock (_events)
            {
                var isNextEvent = _events
                    .TryLast()
                    .Select(x => @event.Version == x.Version + 1)
                    .ValueOrDefault(true);

                if (!isNextEvent)
                    throw new InvalidOperationException("Events cannot be appended with gaps or out of order.");

                _events.Add(@event);

                if (_startVersion == 0)
                    _startVersion = @event.Version;
            }
        }

        public void CommitEvents() { CommitEvents(Int32.MaxValue); }

        public void CommitEvents(Int32 upToVersion)
        {
            lock (_events)
            {
                int maxUncommittedOffset = _events.Count;
                int offset = (upToVersion - _startVersion) + 1;

                _uncommittedOffset = Math.Min(maxUncommittedOffset, offset);
            }
        }

        public IEnumerable<AggregateEvent<TIdentifier>> CommittedEvents
        {
            get { return _events.Take(_uncommittedOffset); }
        }

        public IEnumerable<AggregateEvent<TIdentifier>> UncommittedEvents
        {
            get { return _events.Skip(_uncommittedOffset); }
        }

        public IEnumerable<AggregateEvent<TIdentifier>> Events
        {
            get { return _events; }
        }

        public Boolean IsTruncated
        {
            get { return _startVersion > 1; }
        }

        public Int32 Version
        {
            get { return _events.TryLast().Select(x => x.Version).ValueOrDefault(); }
        }

        public Int32 CommittedVersion
        {
            get { return CommittedEvents.TryLast().Select(x => x.Version).ValueOrDefault(); }
        }
    }
}