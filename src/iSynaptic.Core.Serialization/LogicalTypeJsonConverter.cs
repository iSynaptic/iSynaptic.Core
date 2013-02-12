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
using Newtonsoft.Json;
using iSynaptic.Commons;

namespace iSynaptic.Serialization
{
    public class LogicalTypeJsonConverter : JsonConverter
    {
        private readonly LogicalTypeRegistry _logicalTypeRegistry;

        public LogicalTypeJsonConverter(LogicalTypeRegistry logicalTypeRegistry)
        {
            _logicalTypeRegistry = Guard.NotNull(logicalTypeRegistry, "logicalTypeRegistry");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var typeString = _logicalTypeRegistry
                .TryLookupLogicalType((Type) value)
                .Select(x => x.ToString())
                .ValueOrDefault();

            if(typeString == null)
                throw new InvalidOperationException(String.Format("Unable to find type in logical type registry: '{0}'.", ((Type)value).FullName));

            serializer.Serialize(writer, typeString);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var type = reader.Value.ToMaybe<String>()
                             .SelectMaybe(LogicalType.TryParse)
                             .SelectMaybe(_logicalTypeRegistry.TryLookupActualType);

            if(!type.HasValue)
                throw new InvalidOperationException(String.Format("Unable to find type in logical type registry: {0}.", reader.Value));

            return type.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (Type).IsAssignableFrom(objectType);
        }
    }
}