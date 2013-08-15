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

            if(actualType.IsGenericTypeDefinition)
                throw new ArgumentException("The provided type must not be an open generic.", "actualType");

            lock (_logicalToActualMappings)
            {
                if(!_logicalToActualMappings.TryAdd(logicalType, actualType))
                    throw new ArgumentException(String.Format("Unable to add mapping; the mapping already exists for the provided type: {0}.", logicalType), "logicalType");

                _actualToLogicalMappings.TryAdd(actualType, logicalType);
            }
        }

        public Maybe<Type> TryLookupActualType(LogicalType logicalType)
        {
            return _logicalToActualMappings.TryGetValue(logicalType);
        }

        public Maybe<LogicalType> TryLookupLogicalType(Type type)
        {
            return _actualToLogicalMappings.TryGetValue(type);
        }
    }
}
