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
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using iSynaptic.Commons;

namespace iSynaptic.Serialization
{
    public class OutcomeJsonConverter : JsonConverter
    {
        private static readonly Type _outcomeTypeDefinition =
            typeof(Outcome<>);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var val = (IOutcome) value;

            writer.WriteStartObject();
            writer.WritePropertyName("wasSuccessful");
            serializer.Serialize(writer, val.WasSuccessful);

            var observations = val.Observations.ToArray();
            if (observations.Length > 0)
            {
                writer.WritePropertyName("observations");
                serializer.Serialize(writer, observations);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type observationType = objectType.GetGenericArguments()[0];
            Type arrayType = observationType.MakeArrayType();

            if (reader.TokenType == JsonToken.Null)
                return FormatterServices.GetSafeUninitializedObject(objectType);

            if (reader.TokenType == JsonToken.StartObject)
            {
                Maybe<Boolean> wasSuccessful = Maybe.NoValue;
                Object observations = Array.CreateInstance(observationType, 0);

                while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
                {
                    if (reader.Value.Equals("wasSuccessful"))
                    {
                        reader.Read();
                        wasSuccessful = ((Boolean)serializer.Deserialize(reader, typeof(Boolean))).ToMaybe();
                    }
                    else if (reader.Value.Equals("observations"))
                    {
                        reader.Read();
                        observations = serializer.Deserialize(reader, arrayType);
                    }
                    else
                    {
                        throw new JsonReaderException(String.Format("Unrecognized property: {0}", reader.Value));
                    }
                }

                if (reader.TokenType == JsonToken.EndObject)
                {
                    if (!wasSuccessful.HasValue)
                        throw new JsonReaderException("wasSuccessful was not specified.");

                    return Activator.CreateInstance(objectType, wasSuccessful.Value, observations);
                }
            }

            throw new JsonReaderException("Unable to interpet json.");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType &&
                   !objectType.IsGenericTypeDefinition &&
                   objectType.GetGenericTypeDefinition() == _outcomeTypeDefinition;
        }
    }
}
