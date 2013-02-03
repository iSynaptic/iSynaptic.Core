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
    internal interface IAggregateInternal<out TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        void Initialize(IAggregateMemento memento);
        IEnumerable<IAggregateEvent<TIdentifier>> GetUncommittedEvents();

        void CommitEvents();
    }

    public abstract class Aggregate<TIdentifier> : Aggregate, IAggregate<TIdentifier>, IAggregateInternal<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private AggregateEventStream<TIdentifier> _events;
        private Action<IAggregate<TIdentifier>, IAggregateEvent<TIdentifier>> _dispatcher;

        protected Aggregate()
        {
            Initialize(null);
        }

        private void Initialize(IAggregateMemento memento)
        {
            _events = new AggregateEventStream<TIdentifier>();
            _dispatcher = GetDispatcher<TIdentifier>(GetType());

            OnInitialize();

            if (memento != null)
            {
                var m = memento.ToMemento<TIdentifier>();

                if (m.Snapshot.HasValue)
                    ApplySnapshot(m.Snapshot.Value);

                ApplyEventsCore(m.Events);
            }
        }

        void IAggregateInternal<TIdentifier>.Initialize(IAggregateMemento memento)
        {
            Initialize(memento.ToMemento<TIdentifier>());
        }

        private void ApplySnapshot(IAggregateSnapshot<TIdentifier> snapshot)
        {
            Id = snapshot.Id;
            Version = snapshot.Version;

            OnApplySnapshot(snapshot);
        }

        protected void ApplyEvent(Func<TIdentifier, Int32, IAggregateEvent<TIdentifier>> eventFactory)
        {
            if(Version <= 0)
                throw new InvalidOperationException("This overload of ApplyEvent can only be called after the first event is applied.");

            ApplyEvent(eventFactory(Id, Version + 1));
        }

        protected void ApplyEvent(IAggregateEvent<TIdentifier> @event)
        {
            Guard.NotNull(@event, "event");

            if (_events == null)
                throw new InvalidOperationException("Events cannot be applied until aggregate is initialized. Ensure base constructor is called.");

            ApplyEventsCore(new[] { @event });
        }

        private void ApplyEventsCore(IEnumerable<IAggregateEvent<TIdentifier>> events)
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

        public virtual IAggregateSnapshot<TIdentifier> TakeSnapshot() { return null; }
            
        IEnumerable<IAggregateEvent<TIdentifier>> IAggregateInternal<TIdentifier>.GetUncommittedEvents()
        {
            return _events.UncommittedEvents;
        }

        public IEnumerable<IAggregateEvent<TIdentifier>> GetEvents() { return _events.Events; }

        void IAggregateInternal<TIdentifier>.CommitEvents()
        {
            _events.CommitEvents();
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnApplySnapshot(IAggregateSnapshot<TIdentifier> snapshot) { }
        
        public TIdentifier Id { get; private set; }
        public Int32 Version { get; private set; }
    }

    public abstract class Aggregate
    {
        private static readonly TypeHierarchyComparer _typeHierarchyComparer
            = new TypeHierarchyComparer();

        private static readonly ConcurrentDictionary<Type, Delegate> _dispatchers
            = new ConcurrentDictionary<Type, Delegate>();

        internal static Action<IAggregate<TIdentifier>, IAggregateEvent<TIdentifier>> GetDispatcher<TIdentifier>(Type aggregateType)
            where TIdentifier : IEquatable<TIdentifier>
        {
            var baseAggregateType = typeof(IAggregate<TIdentifier>);

            return (Action<IAggregate<TIdentifier>, IAggregateEvent<TIdentifier>>)_dispatchers.GetOrAdd(aggregateType, t =>
            {
                var eventType = typeof(IAggregateEvent<TIdentifier>);
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
                    return baseDispatcher ?? (Delegate)(Action<IAggregate<TIdentifier>, IAggregateEvent<TIdentifier>>)((a, e) => { });

                if (baseDispatcher != null)
                    applicators = applicators.Concat(new[] { Expression.Call(baseDispatcher.GetMethodInfo(), aggregateParam, eventParam) }).ToArray();

                return Expression.Lambda<Action<IAggregate<TIdentifier>, IAggregateEvent<TIdentifier>>>(Expression.Block(applicators), aggregateParam, eventParam)
                    .Compile();
            });
        }

        internal Aggregate() { }
    }
}