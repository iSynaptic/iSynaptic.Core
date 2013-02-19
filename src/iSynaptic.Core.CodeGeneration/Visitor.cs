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

namespace iSynaptic.CodeGeneration
{
    public abstract class Visitor : Visitor<Object>
    {
        public void Dispatch<T>(IEnumerable<T> subjects, Action<T, T> interleave)
            where T : IVisitable
        {
            Dispatch(subjects, null, (st, l, r) =>
            {
                interleave(l, r);
                return st;
            });
        }
    }

    public abstract class Visitor<TState> : IVisitor<TState>
    {
        public delegate TState VisitorDispatcher(Visitor<TState> visitor, IVisitable subject, TState state);

        private static readonly TypeHierarchyComparer _typeHierarchyComparer
            = new TypeHierarchyComparer();

        private static readonly ConcurrentDictionary<Type, VisitorDispatcher> _dispatchers
            = new ConcurrentDictionary<Type, VisitorDispatcher>();

        private readonly VisitorDispatcher _dispatcher;

        protected Visitor()
        {
            _dispatcher = GetDispatcher(GetType());
        }

        internal static VisitorDispatcher GetDispatcher(Type visitorType)
        {
            var baseVisitorType = typeof(Visitor<TState>);

            return _dispatchers.GetOrAdd(visitorType, t =>
            {
                var subjectType = typeof(IVisitable);
                var stateType = typeof(TState);

                var baseDispatcher = baseVisitorType.IsAssignableFrom(t.BaseType)
                                         ? GetDispatcher(t.BaseType)
                                         : null;

                var visitorParam = Expression.Parameter(baseVisitorType, "visitor");
                var subjectParam = Expression.Parameter(subjectType, "subject");
                var stateParam = Expression.Parameter(stateType, "state");

                var returnLabel = Expression.Label(stateType);

                var applicators = t
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(x => x.DeclaringType == t)
                    .Select(m => new { Method = m, Parameters = m.GetParameters() })
                    .Select(x => new { x.Method, x.Parameters, ParameterCount = x.Parameters.Length })
                    .Where(x => x.Method.Name.StartsWith("Visit") && (x.ParameterCount == 1 || x.ParameterCount == 2))
                    .Select(x => new
                    {
                        x.Method,
                        SubjectType = x.Parameters[0].ParameterType,
                        StateType = x.ParameterCount == 2
                                        ? x.Parameters[1].ParameterType
                                        : null
                    })
                    .Where(x => subjectType.IsAssignableFrom(x.SubjectType))
                    .Where(x => x.StateType == null || stateType.IsAssignableFrom(x.StateType))
                    .Where(x => (x.Method.ReturnType == typeof(void)) ||
                                 (x.StateType != null && stateType.IsAssignableFrom(x.Method.ReturnType)))
                    .OrderByDescending(x => x.SubjectType, _typeHierarchyComparer)
                    .ThenByDescending(x => x.StateType, _typeHierarchyComparer)
                    .Select(x =>
                    {
                        var subjectTypeTest = Expression.TypeIs(subjectParam, x.SubjectType);

                        var typeTest = x.StateType != null
                                           ? (Expression)Expression.AndAlso(subjectTypeTest,
                                                Expression.OrElse(
                                                    Expression.TypeIs(stateParam, x.StateType),
                                                    Expression.Equal(stateParam, Expression.Default(stateType))
                                                ))
                                           : subjectTypeTest;

                        var subjectArgument = Expression.Convert(subjectParam, x.SubjectType);
                        var arguments = x.StateType != null
                                            ? new[] { subjectArgument, Expression.Convert(stateParam, x.StateType) }
                                            : new[] { subjectArgument };

                        var callMethod = Expression.Call(
                            Expression.Convert(visitorParam, t),
                            x.Method,
                            arguments);

                        if (x.Method.ReturnType == typeof(void))
                        {
                            return Expression.IfThen(typeTest,
                                    Expression.Block(callMethod, Expression.Return(returnLabel, stateParam, stateType)));
                        }

                        return Expression.IfThen(typeTest,
                                Expression.Return(returnLabel,
                                    Expression.Convert(
                                        callMethod,
                                        stateType),
                                    stateType));
                    })
                    .OfType<Expression>()
                    .ToArray();

                if (applicators.Length <= 0)
                    return baseDispatcher ?? (VisitorDispatcher)((v, s, st) => { throw new InvalidOperationException("Could not find method to dispatch to."); });

                if (baseDispatcher != null)
                    applicators = applicators.Concat(new[]
                    {
                        Expression.Return(returnLabel, 
                                          Expression.Call(baseDispatcher.GetMethodInfo(), visitorParam, subjectParam, stateParam),
                                          stateType)
                    }).ToArray();

                return Expression.Lambda<VisitorDispatcher>(
                    Expression.Label(returnLabel, Expression.Block(stateType, applicators)), visitorParam, subjectParam, stateParam)
                                 .Compile();
            });
        }

        public void Dispatch(IEnumerable<IVisitable> subjects)
        {
            Dispatch(subjects, default(TState), (st, l, r) => st);
        }

        public TState Dispatch(IEnumerable<IVisitable> subjects, TState state)
        {
            return Dispatch(subjects, state, (st, l, r) => st);
        }

        public TState Dispatch<T>(IEnumerable<T> subjects, TState state, Action<TState, T, T> interleave)
            where T : IVisitable
        {
            return Dispatch(subjects, state, (st, l, r) =>
            {
                interleave(st, l, r);
                return st;
            });
        }

        public TState Dispatch<T>(IEnumerable<T> subjects, TState state, Func<TState, T, T, TState> interleave)
            where T : IVisitable
        {
            Guard.NotNull(subjects, "subjects");
            Guard.NotNull(interleave, "interleave");

            Maybe<T> previous = Maybe.NoValue;

            foreach (var subject in subjects)
            {
                if (NotInterestedIn(subject, state))
                    continue;

                if (previous.HasValue)
                    state = interleave(state, previous.Value, subject);

                state = _dispatcher(this, subject, state);

                previous = subject.ToMaybe();
            }

            return state;
        }

        protected TState DispatchChildren(IVisitable subject)
        {
            return DispatchChildren(subject, default(TState));
        }

        protected TState DispatchChildren(IVisitable subject, TState state)
        {
            return DispatchChildren(subject, state, (st, l, r) => st);
        }

        protected TState DispatchChildren(IVisitable subject, TState state, Func<TState, IVisitable, IVisitable, TState> interleave)
        {
            Guard.NotNull(subject, "subject");
            Guard.NotNull(interleave, "interleave");

            TState result = default(TState);

            subject.AcceptChildren(children => result = Dispatch(children, state, interleave));
            return result;
        }

        protected virtual Boolean NotInterestedIn(IVisitable subject, TState state) { return false; }
    }

}
