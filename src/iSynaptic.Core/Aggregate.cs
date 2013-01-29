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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using iSynaptic.Commons;
using iSynaptic.Commons.Reflection;

namespace iSynaptic
{
    public abstract class Aggregate
    {
        private static readonly TypeHierarchyComparer _typeHierarchyComparer
            = new TypeHierarchyComparer();

        private static readonly ConcurrentDictionary<Type, Delegate> _dispatchers
            = new ConcurrentDictionary<Type, Delegate>();

        internal static Action<Aggregate<TIdentifier>, AggregateEvent<TIdentifier>> GetDispatcher<TIdentifier>(Type aggregateType)
            where TIdentifier : IEquatable<TIdentifier>
        {
            var baseAggregateType = typeof (Aggregate<TIdentifier>);

            return (Action<Aggregate<TIdentifier>, AggregateEvent<TIdentifier>>)_dispatchers.GetOrAdd(aggregateType, t =>
            {
                var eventType = typeof (AggregateEvent<TIdentifier>);
                var baseDispatcher = baseAggregateType.IsAssignableFrom(t.BaseType)
                    ? GetDispatcher<TIdentifier>(t.BaseType)
                    : null;

                var aggregateParam = Expression.Parameter(baseAggregateType);
                var eventParam = Expression.Parameter(eventType);

                var applicators = t
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(x => x.DeclaringType == t)
                    .Select(m => new { Method = m, Parameters = m.GetParameters() })
                    .Where(x => x.Method.Name == "On" && x.Parameters.Length == 1)
                    .Select(x => new { x.Method, x.Parameters[0].ParameterType })
                    .Where(x => eventType.IsAssignableFrom(x.ParameterType))
                    .OrderByDescending(x => x.ParameterType, _typeHierarchyComparer)
                    .Select(x => 
                        Expression.IfThen(
                            Expression.TypeIs(eventParam, x.ParameterType),
                            Expression.Call(
                                Expression.Convert(aggregateParam, t),
                                x.Method,
                                Expression.Convert(eventParam, x.ParameterType))))
                    .OfType<Expression>()
                    .ToArray();

                if (applicators.Length <= 0)
                    return baseDispatcher ?? (Delegate)(Action<Aggregate<TIdentifier>, AggregateEvent<TIdentifier>>)((a, e) => { });

                if (baseDispatcher != null)
                    applicators = applicators.Concat(new[]{Expression.Call(baseDispatcher.GetMethodInfo(), aggregateParam, eventParam)}).ToArray();

                return Expression.Lambda<Action<Aggregate<TIdentifier>, AggregateEvent<TIdentifier>>>(Expression.Block(applicators), aggregateParam, eventParam)
                    .Compile();
            });
        }

        internal Aggregate() { }
    }

    public abstract class Aggregate<TIdentifier> : Aggregate
        where TIdentifier : IEquatable<TIdentifier>
    {
        private AggregateEventStream<TIdentifier> _events;
        private Action<Aggregate<TIdentifier>, AggregateEvent<TIdentifier>> _dispatcher;

        protected Aggregate()
        {
            Initialize(AggregateMemento<TIdentifier>.Empty);
        }

        internal void Initialize(AggregateMemento<TIdentifier> memento)
        {
            _events = new AggregateEventStream<TIdentifier>();
            _dispatcher = GetDispatcher<TIdentifier>(GetType());

            OnInitialize();

            if (!memento.IsEmpty)
            {
                if (memento.Snapshot.HasValue)
                    ApplySnapshot(memento.Snapshot.Value);

                ApplyEventsCore(memento.Events);
            }
        }

        private void ApplySnapshot(AggregateSnapshot<TIdentifier> snapshot)
        {
            Id = snapshot.Id;
            Version = snapshot.Version;

            OnApplySnapshot(snapshot);
        }

        protected void ApplyEvent(AggregateEvent<TIdentifier> @event)
        {
            Guard.NotNull(@event, "event");

            if (_events == null)
                throw new InvalidOperationException("Events cannot be applied until aggregate is initialized. Ensure base constructor is called.");

            ApplyEventsCore(new[] { @event });
        }

        private void ApplyEventsCore(IEnumerable<AggregateEvent<TIdentifier>> events)
        {
            lock (events)
            {
                foreach (var @event in events)
                {
                    if (_events.Version <= 0)
                        Id = @event.Id;

                    Version = @event.Version;

                    _dispatcher(this, @event);
                    _events.AppendEvent(@event);
                }
            }
        }

        internal AggregateSnapshot<TIdentifier> TakeSnapshot()
        {
            return OnTakeSnapshot();
        }

        internal IEnumerable<AggregateEvent<TIdentifier>> GetUncommittedEvents()
        {
            return _events.UncommittedEvents;
        }

        internal void CommitEvents()
        {
            _events.CommitEvents();
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnApplySnapshot(AggregateSnapshot<TIdentifier> snapshot) { }
        protected virtual AggregateSnapshot<TIdentifier> OnTakeSnapshot() { return null; }

        public TIdentifier Id { get; private set; }
        public Int32 Version { get; private set; }

        public IEnumerable<AggregateEvent<TIdentifier>> GetEvents() { return _events.Events; }
    }
}