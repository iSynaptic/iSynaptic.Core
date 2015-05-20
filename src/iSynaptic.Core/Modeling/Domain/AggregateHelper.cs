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
using iSynaptic.Commons.Linq;
using iSynaptic.Commons.Reflection;

namespace iSynaptic.Modeling.Domain
{
    internal static class AggregateHelper
    {
        private static readonly TypeHierarchyComparer _typeHierarchyComparer
            = new TypeHierarchyComparer();

        private static readonly ConcurrentDictionary<Type, Delegate> _eventDispatchers
            = new ConcurrentDictionary<Type, Delegate>();

        private static readonly ConcurrentDictionary<Type, Delegate> _snapshotDispatchers
            = new ConcurrentDictionary<Type, Delegate>();

        private static readonly ConcurrentDictionary<Type, Delegate> _resetOperations
            = new ConcurrentDictionary<Type, Delegate>();

        public static Action<IAggregate<TIdentifier>, IAggregateEvent<TIdentifier>> GetEventDispatcher<TIdentifier>(Type aggregateType)
            where TIdentifier : IEquatable<TIdentifier>
        {
            return GetDispatcher<IAggregateEvent<TIdentifier>, TIdentifier>(aggregateType, "On", _eventDispatchers);
        }

        public static Action<IAggregate<TIdentifier>, IAggregateSnapshot> GetSnapshotDispatcher<TIdentifier>(Type aggregateType)
            where TIdentifier : IEquatable<TIdentifier>
        {
            return GetDispatcher<IAggregateSnapshot, TIdentifier>(aggregateType, "Apply", _snapshotDispatchers);
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
                var aggregateVariable = Expression.Variable(t);

                var assignAggregateVariable = Expression.Assign(aggregateVariable, Expression.Convert(aggregateParam, t));

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
                                    aggregateVariable,
                                    x.Method,
                                    Expression.Convert(inputParam, x.ParameterType))))
                    .OfType<Expression>()
                    .ToArray();

                if (applicators.Length <= 0)
                    return baseDispatcher ?? (Delegate)(Action<IAggregate<TIdentifier>, T>)((a, s) => { });

                if (baseDispatcher != null)
                    applicators = applicators.Concat(new[] { Expression.Invoke(Expression.Constant(baseDispatcher), aggregateParam, inputParam) }).ToArray();

                return Expression.Lambda<Action<IAggregate<TIdentifier>, T>>(Expression.Block(new[]{ aggregateVariable }, new[] { assignAggregateVariable }.Concat(applicators)), aggregateParam, inputParam)
                                 .Compile();
            });
        }

        public static Action<IAggregate<TIdentifier>> GetResetOperation<TIdentifier>(Type aggregateType)
            where TIdentifier : IEquatable<TIdentifier>
        {
            var baseAggregateType = typeof(IAggregate<TIdentifier>);

            return (Action<IAggregate<TIdentifier>>)_resetOperations.GetOrAdd(aggregateType, t =>
            {
                var baseResetOperation = baseAggregateType.IsAssignableFrom(t.BaseType)
                                             ? GetResetOperation<TIdentifier>(t.BaseType)
                                             : null;

                var aggregateParam = Expression.Parameter(baseAggregateType);
                var aggregateVariable = Expression.Variable(t);

                var assignVariable = Expression.Assign(
                    aggregateVariable,
                    Expression.Convert(aggregateParam, t));

                var immunityPredicate = ((Expression<Func<object>>) (() => Aggregate.FieldResetImmunityPredicate)).Body;

                IEnumerable<Expression> resetExpressions = t
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => x.DeclaringType == t)
                    .Unless(x => x.IsInitOnly)
                    .Unless(x => x.IsDefined(typeof(ImmuneToResetAttribute), true))
                    .Select(x =>
                    {
                        var test = Expression.Not(Expression.Invoke(immunityPredicate, Expression.Constant(x)));

                        return Expression.IfThen(test,
                            Expression.Assign(
                                Expression.Field(assignVariable, x),
                                Expression.Default(x.FieldType)));
                    });

                if (baseResetOperation != null)
                    resetExpressions = resetExpressions.Concat(new[] { Expression.Invoke(Expression.Constant(baseResetOperation), aggregateParam) }).ToArray();

                return Expression.Lambda<Action<IAggregate<TIdentifier>>>(Expression.Block(new[] { aggregateVariable }, new[] { assignVariable }.Concat(resetExpressions)), aggregateParam)
                                 .Compile();
            });
        }
    }
}