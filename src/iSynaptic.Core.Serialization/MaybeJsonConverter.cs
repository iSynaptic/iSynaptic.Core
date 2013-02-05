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
using Newtonsoft.Json;
using iSynaptic.Commons;

namespace iSynaptic.Serialization
{
    public class MaybeJsonConverter : JsonConverter
    {
        private static readonly Type _maybeTypeDefinition =
            typeof (Maybe<>);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var val = (IMaybe) value;
            
            if (val.HasValue)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("value");
                serializer.Serialize(writer, val.Value);
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteStartObject();
                writer.WritePropertyName("hasValue");
                writer.WriteValue(false);
                writer.WriteEndObject();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type actualType = objectType.GetGenericArguments()[0];

            if (reader.TokenType == JsonToken.Null)
                return FormatterServices.GetSafeUninitializedObject(objectType);

            if (reader.TokenType == JsonToken.StartObject)
            {
                reader.Read();
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    if (reader.Value.Equals("value"))
                    {
                        reader.Read();
                        var val = serializer.Deserialize(reader, actualType);
                        reader.Read();

                        return Activator.CreateInstance(objectType, new[] {val});
                    }
                    else if (reader.Value.Equals("hasValue"))
                    {
                        reader.Read();
                        reader.Read();

                        return FormatterServices.GetSafeUninitializedObject(objectType);
                    }
                }
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType &&
                   !objectType.IsGenericTypeDefinition &&
                   objectType.GetGenericTypeDefinition() == _maybeTypeDefinition;
        }
    }
}