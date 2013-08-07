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
    public abstract class MessageHandler
    {
        private static readonly TypeHierarchyComparer _typeHierarchyComparer
            = new TypeHierarchyComparer();

        private static readonly ConcurrentDictionary<Type, Delegate> _projectionDispatchers
            = new ConcurrentDictionary<Type, Delegate>();

        protected virtual void Handle(IEnumerable<Object> messages)
        {
            Guard.NotNull(messages, "messages");

            foreach (var message in messages)
                Handle(message);
        }

        protected virtual void Handle(Object message)
        {
            Guard.NotNull(message, "message");

            if (!ShouldHandle(message))
                return;

            var dispatcher = GetDispatcher(GetType(), "On", _projectionDispatchers);
            dispatcher(this, message);
        }

        protected virtual bool ShouldHandle(Object message)
        {
            return true;
        }
        
        private static Action<MessageHandler, Object> GetDispatcher(Type handlerType, String methodName, ConcurrentDictionary<Type, Delegate> dictionary)
        {
            var baseHandlerType = typeof(MessageHandler);

            return (Action<MessageHandler, Object>)dictionary.GetOrAdd(handlerType, t =>
            {
                var methodEndTarget = Expression.Label();
                var methodEnd = Expression.Label(methodEndTarget);

                var paramType = typeof(Object);
                var baseDispatcher = baseHandlerType.IsAssignableFrom(t.BaseType)
                                         ? GetDispatcher(t.BaseType, methodName, dictionary)
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
                            Expression.Block(
                                Expression.Call(
                                    handlerVariable,
                                    x.Method,
                                    Expression.Convert(inputParam, x.ParameterType)
                                ),
                                Expression.Return(methodEndTarget)
                            )
                        )
                    )
                    .OfType<Expression>()
                    .ToArray();

                if (applicators.Length <= 0)
                    return baseDispatcher ?? (Delegate)(Action<MessageHandler, Object>)((h, m) => h.HandleUnexpectedMessage(m));

                if (baseDispatcher != null)
                    applicators = applicators.Concat(new[] { Expression.Invoke(Expression.Constant(baseDispatcher), handlerParam, inputParam) }).ToArray();

                return Expression.Lambda<Action<MessageHandler, Object>>(Expression.Block(new[] { handlerVariable }, new[] { assignHandlerVariable }.Concat(applicators).Concat(new Expression[]{ methodEnd })), handlerParam, inputParam)
                                 .Compile();
            });
        }

        protected virtual void HandleUnexpectedMessage(Object message)
        {
            throw new InvalidOperationException(String.Format("Unable to handle message of type '{0}'.", message.GetType().FullName));
        }
    }
}
