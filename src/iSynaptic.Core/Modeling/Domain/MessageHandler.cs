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
using iSynaptic.Commons.Reflection;

namespace iSynaptic.Modeling.Domain
{
    public abstract class MessageHandler
    {
        private static readonly TypeHierarchyComparer _typeHierarchyComparer
            = new TypeHierarchyComparer();

        private static readonly ConcurrentDictionary<Type, Delegate> _projectionDispatchers
            = new ConcurrentDictionary<Type, Delegate>();

        protected virtual void Handle<T>(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                Handle(value);
            }
        }

        protected virtual void Handle<T>(T value)
        {
            var dispatcher = GetDispatcher<T>(GetType(), "On", _projectionDispatchers);
            dispatcher(this, value);
        }
        
        private static Action<MessageHandler, T> GetDispatcher<T>(Type handlerType, String methodName, ConcurrentDictionary<Type, Delegate> dictionary)
        {
            var baseHandlerType = typeof(MessageHandler);

            return (Action<MessageHandler, T>)dictionary.GetOrAdd(handlerType, t =>
            {
                var paramType = typeof(T);
                var baseDispatcher = baseHandlerType.IsAssignableFrom(t.BaseType)
                                         ? GetDispatcher<T>(t.BaseType, methodName, dictionary)
                                         : null;

                var handlerParam = Expression.Parameter(baseHandlerType);
                var inputParam = Expression.Parameter(paramType);
                var handlerVariable = Expression.Variable(t);

                var assignHandlerVariable = Expression.Assign(handlerVariable, Expression.Convert(handlerParam, t));

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
                                    handlerVariable,
                                    x.Method,
                                    Expression.Convert(inputParam, x.ParameterType))))
                    .OfType<Expression>()
                    .ToArray();

                if (applicators.Length <= 0)
                    return baseDispatcher ?? (Delegate)(Action<MessageHandler, T>)((p, s) => { });

                if (baseDispatcher != null)
                    applicators = applicators.Concat(new[] { Expression.Invoke(Expression.Constant(baseDispatcher), handlerParam, inputParam) }).ToArray();

                return Expression.Lambda<Action<MessageHandler, T>>(Expression.Block(new[] { handlerVariable }, new[] { assignHandlerVariable }.Concat(applicators)), handlerParam, inputParam)
                                 .Compile();
            });
        }

    }
}
