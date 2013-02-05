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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using iSynaptic.Commons;
using iSynaptic.Commons.Collections.Generic;
using iSynaptic.Commons.Linq;

namespace iSynaptic.Serialization
{
    public class InMemoryJsonAggregateRepository<TAggregate, TIdentifier> : MementoBasedAggregateRepository<TAggregate, TIdentifier>
        where TAggregate : class, IAggregate<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly Dictionary<TIdentifier, String> _state =
            new Dictionary<TIdentifier, String>();

        private readonly Task _completedTask;

        private readonly JsonSerializer _serializer;

        public InMemoryJsonAggregateRepository(JsonSerializer serializer)
        {
            _serializer = Guard.NotNull(serializer, "serializer");

            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            _completedTask = tcs.Task;
        }

        protected override Maybe<AggregateMemento<TIdentifier>> TryLoadMemento(TIdentifier id)
        {
            lock (_state)
            {
                return _state.TryGetValue(id)
                             .Select(json => _serializer.Deserialize<AggregateMemento<TIdentifier>>(json));
            }
        }

        protected override void StoreMemento(Func<KeyValuePair<TIdentifier, AggregateMemento<TIdentifier>>> mementoFactory)
        {
            lock (_state)
            {
                var memento = mementoFactory();
                _state[memento.Key] = _serializer.Serialize(memento.Value);
            }
        }
    }
}
