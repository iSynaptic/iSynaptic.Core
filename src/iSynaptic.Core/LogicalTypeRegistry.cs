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
using iSynaptic.Commons;
using iSynaptic.Commons.Collections.Concurrent;
using System.Linq;
using iSynaptic.Commons.Linq;

namespace iSynaptic
{
    public class LogicalTypeRegistry : ILogicalTypeRegistry
    {
        private readonly ConcurrentDictionary<LogicalType, Type> _logicalToActualMappings =
            new ConcurrentDictionary<LogicalType, Type>();

        private readonly ConcurrentDictionary<Type, LogicalType> _actualToLogicalMappings =
            new ConcurrentDictionary<Type, LogicalType>();

        public void AddMapping(LogicalType logicalType, Type actualType)
        {
            Guard.NotNull(logicalType, "logicalType");
            Guard.NotNull(actualType, "actualType");

            if(logicalType.IsOpenType != actualType.IsGenericTypeDefinition)
            {
                throw logicalType.IsOpenType
                    ? new ArgumentException("Actual type must be a generic type definition.", "actualType")
                    : new ArgumentException("Actual type must not be a generic type definition.", "actualType");
            }

            if (logicalType.IsOpenType && logicalType.Arity != actualType.GetGenericArguments().Length)
            {
                throw new ArgumentException("The number of generic type parameters does not match the logical type arity.", "actualType");
            }

            lock (_logicalToActualMappings)
            {
                if(_logicalToActualMappings.ContainsKey(logicalType) || _actualToLogicalMappings.ContainsKey(actualType))
                    throw new ArgumentException("Unable to add mapping; the mapping already exists for the provided types.", "logicalType");

                _logicalToActualMappings.TryAdd(logicalType, actualType);
                _actualToLogicalMappings.TryAdd(actualType, logicalType);
            }
        }

        public Maybe<Type> TryLookupActualType(LogicalType logicalType)
        {
            var result = _logicalToActualMappings.TryGetValue(logicalType);
            if(!result.HasValue && !logicalType.IsOpenType)
            {
                var openActualType = _logicalToActualMappings.TryGetValue(logicalType.GetOpenType());
                if (!openActualType.HasValue) return Maybe.NoValue;

                var typeArguments = logicalType.TypeArguments
                    .Select(x => TryLookupActualType(x))
                    .Squash()
                    .ToArray();

                var actualType = openActualType.Value;

                if (typeArguments.Length == actualType.GetGenericArguments().Length)
                    return actualType.MakeGenericType(typeArguments).ToMaybe();
            }

            return result;
        }

        public Maybe<LogicalType> TryLookupLogicalType(Type actualType)
        {
            var result = _actualToLogicalMappings.TryGetValue(actualType);
            if (!result.HasValue && actualType.IsGenericType && !actualType.IsGenericTypeDefinition)
            {
                var typeArgs = actualType.GetGenericArguments();

                var openLogicalType = _actualToLogicalMappings.TryGetValue(actualType.GetGenericTypeDefinition());
                if (!openLogicalType.HasValue) return Maybe.NoValue;

                var typeArguments = typeArgs
                    .Select(x => TryLookupLogicalType(x))
                    .Squash()
                    .ToArray();

                var logicalType = openLogicalType.Value;

                if (typeArguments.Length == logicalType.Arity)
                    return logicalType.MakeClosedType(typeArguments).ToMaybe();
            }

            return result;
        }
    }
}
