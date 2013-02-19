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

namespace iSynaptic.Modeling.Domain
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
        private Action<IAggregate<TIdentifier>, IAggregateEvent<TIdentifier>> _eventDispatcher;
        private Action<IAggregate<TIdentifier>, IAggregateSnapshot<TIdentifier>> _snapshotDispatcher;

        protected Aggregate()
        {
            Initialize(null);
        }

        private void Initialize(IAggregateMemento memento)
        {
            _events = new AggregateEventStream<TIdentifier>();
            _eventDispatcher = GetEventDispatcher<TIdentifier>(GetType());
            _snapshotDispatcher = GetSnapshotDispatcher<TIdentifier>(GetType());

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

            _snapshotDispatcher(this, snapshot);
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

                    _eventDispatcher(this, @event);
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
        
        public TIdentifier Id { get; private set; }
        public Int32 Version { get; private set; }
    }

    public abstract class Aggregate
    {
        private static readonly TypeHierarchyComparer _typeHierarchyComparer
            = new TypeHierarchyComparer();

        private static readonly ConcurrentDictionary<Type, Delegate> _eventDispatchers
            = new ConcurrentDictionary<Type, Delegate>();

        private static readonly ConcurrentDictionary<Type, Delegate> _snapshotDispatchers
            = new ConcurrentDictionary<Type, Delegate>();

        internal static Action<IAggregate<TIdentifier>, IAggregateEvent<TIdentifier>> GetEventDispatcher<TIdentifier>(Type aggregateType)
            where TIdentifier : IEquatable<TIdentifier>
        {
            return GetDispatcher<IAggregateEvent<TIdentifier>, TIdentifier>(aggregateType, "On", _eventDispatchers);
        }

        internal static Action<IAggregate<TIdentifier>, IAggregateSnapshot<TIdentifier>> GetSnapshotDispatcher<TIdentifier>(Type aggregateType)
            where TIdentifier : IEquatable<TIdentifier>
        {
            return GetDispatcher<IAggregateSnapshot<TIdentifier>, TIdentifier>(aggregateType, "Apply", _snapshotDispatchers);
        }

        private static Action<IAggregate<TIdentifier>, T> GetDispatcher<T, TIdentifier>(Type aggregateType, String methodName, ConcurrentDictionary<Type, Delegate> dictionary)
            where TIdentifier : IEquatable<TIdentifier>
        {
            var baseAggregateType = typeof(IAggregate<TIdentifier>);

            return (Action<IAggregate<TIdentifier>, T>)dictionary.GetOrAdd(aggregateType, t =>
            {
                var paramType = typeof(T);
                var baseDispatcher = baseAggregateType.IsAssignableFrom(t.BaseType)
                    ? GetDispatcher<T, TIdentifier>(t.BaseType, methodName, dictionary)
                    : null;

                var aggregateParam = Expression.Parameter(baseAggregateType);
                var inputParam = Expression.Parameter(paramType);

                var applicators = t
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(x => x.DeclaringType == t)
                    .Select(m => new { Method = m, Parameters = m.GetParameters() })
                    .Where(x => x.Method.Name == methodName && x.Parameters.Length == 1)
                    .Select(x => new { x.Method, x.Parameters[0].ParameterType })
                    .Where(x => paramType.IsAssignableFrom(x.ParameterType))
                    .OrderByDescending(x => x.ParameterType, _typeHierarchyComparer)
                    .Select(x =>
                        Expression.IfThen(
                            Expression.TypeIs(inputParam, x.ParameterType),
                            Expression.Call(
                                Expression.Convert(aggregateParam, t),
                                x.Method,
                                Expression.Convert(inputParam, x.ParameterType))))
                    .OfType<Expression>()
                    .ToArray();

                if (applicators.Length <= 0)
                    return baseDispatcher ?? (Delegate)(Action<IAggregate<TIdentifier>, T>)((a, s) => { });

                if (baseDispatcher != null)
                    applicators = applicators.Concat(new[] { Expression.Call(baseDispatcher.GetMethodInfo(), aggregateParam, inputParam) }).ToArray();

                return Expression.Lambda<Action<IAggregate<TIdentifier>, T>>(Expression.Block(applicators), aggregateParam, inputParam)
                    .Compile();
            });
        }

        internal Aggregate() { }
    }
}