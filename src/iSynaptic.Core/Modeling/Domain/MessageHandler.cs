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
using System.Threading.Tasks;
using iSynaptic.Commons;
using iSynaptic.Commons.Reflection;

namespace iSynaptic.Modeling.Domain
{
    using Dispatcher = Func<MessageHandler, Object, Task>;

    public abstract class MessageHandler
    {
        private static readonly Task _completedTask;

        private static readonly TypeHierarchyComparer _typeHierarchyComparer
            = new TypeHierarchyComparer();

        private static readonly ConcurrentDictionary<Type, Dispatcher> _projectionDispatchers
            = new ConcurrentDictionary<Type, Dispatcher>();

        static MessageHandler()
        {
            _completedTask = Task.FromResult(true);
        }

        protected virtual async Task Handle(IEnumerable<Object> messages)
        {
            Guard.NotNull(messages, "messages");

            foreach (var message in messages)
                await Handle(message);
        }

        protected virtual Task Handle(Object message)
        {
            Guard.NotNull(message, "message");

            if (!ShouldHandle(message))
                return _completedTask;

            var dispatcher = GetDispatcher(GetType(), "On", _projectionDispatchers);
            return dispatcher(this, message);
        }

        protected virtual bool ShouldHandle(Object message)
        {
            return true;
        }

        private static Dispatcher GetDispatcher(Type handlerType, String methodName, ConcurrentDictionary<Type, Dispatcher> dictionary)
        {
            var baseHandlerType = typeof(MessageHandler);

            return dictionary.GetOrAdd(handlerType, t =>
            {
                var returnLabel = Expression.Label(typeof (Task));

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
                    .Where(x => x.Method.ReturnType == typeof(void) || x.Method.ReturnType == typeof(Task))
                    .Select(x => new { x.Method, x.Parameters[0].ParameterType, ReturnsTask = x.Method.ReturnType == typeof(Task) })
                    .Where(x => paramType.IsAssignableFrom(x.ParameterType))
                    .OrderByDescending(x => x.ParameterType, _typeHierarchyComparer)
                    .Select(x =>
                    {
                        var call = Expression.Call(
                            handlerVariable,
                            x.Method,
                            Expression.Convert(inputParam, x.ParameterType)
                        );

                        var applicatorBody = x.ReturnsTask
                            ? (Expression)Expression.Return(returnLabel, call, typeof(Task))
                            : Expression.Block(call, Expression.Return(returnLabel, Expression.Constant(_completedTask), typeof(Task)));

                        return Expression.IfThen(Expression.TypeIs(inputParam, x.ParameterType), applicatorBody);
                    })
                    .OfType<Expression>()
                    .ToArray();

                if (applicators.Length <= 0)
                    return baseDispatcher ?? (Dispatcher)((h, m) => h.HandleUnexpectedMessage(m));

                if (baseDispatcher != null)
                    applicators = applicators.Concat(new[] { Expression.Return(returnLabel, Expression.Invoke(Expression.Constant(baseDispatcher), handlerParam, inputParam), typeof(Task)) }).ToArray();

                var body = new[] { assignHandlerVariable }.Concat(applicators);

                return Expression.Lambda<Dispatcher>(
                    Expression.Label(returnLabel, Expression.Block(typeof(Task), new[] { handlerVariable }, body)),
                                     handlerParam, inputParam)
                    .Compile();
            });
        }

        protected virtual Task HandleUnexpectedMessage(Object message)
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetException(new InvalidOperationException(String.Format("Unable to handle message of type '{0}'.", message.GetType().FullName)));
            return tcs.Task;
        }
    }
}
