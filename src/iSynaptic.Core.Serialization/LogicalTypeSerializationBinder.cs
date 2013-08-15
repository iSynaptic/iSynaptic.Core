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
using System.Runtime.Serialization;
using iSynaptic.Commons;

namespace iSynaptic.Serialization
{
    public class LogicalTypeSerializationBinder : SerializationBinder
    {
        private readonly ILogicalTypeRegistry _logicalTypeRegistry;

        public LogicalTypeSerializationBinder(ILogicalTypeRegistry logicalTypeRegistry)
        {
            _logicalTypeRegistry = Guard.NotNull(logicalTypeRegistry, "logicalTypeRegistry");
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            var type = LogicalType.TryParse(typeName)
                                  .SelectMaybe(_logicalTypeRegistry.TryLookupActualType);

            if (!type.HasValue)
            {
                String formatString = "Unable to find type in logical type registry: " + 
                    (assemblyName != null
                        ? "{0}.{1}"
                        : "{1}");

                throw new InvalidOperationException(String.Format(formatString, assemblyName, typeName));
            }

            return type.Value;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = _logicalTypeRegistry.TryLookupLogicalType(serializedType)
                                           .Select(x => x.ToString())
                                           .ValueOrDefault();

            if (typeName == null)
                throw new InvalidOperationException(String.Format("Unable to find type in logical type registry: '{0}'.", serializedType.FullName));
        }
    }
}