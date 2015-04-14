// The MIT License
// 
// Copyright (c) 2015 Jordan E. Terrell
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using iSynaptic.Commons.Reflection;
using iSynaptic.Modeling.Domain;
using Newtonsoft.Json;

namespace iSynaptic.Serialization
{
    public class ScalarValueConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var val = (IScalarValue) value;
            serializer.Serialize(writer, val.Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var underlyingType = GetUnderlyingType(objectType);
            var value = serializer.Deserialize(reader, underlyingType);

            return Activator.CreateInstance(objectType, value);
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsGenericTypeDefinition || !objectType.DoesImplementType(typeof (IScalarValue<>)))
                return false;

            var underlyingType = GetUnderlyingType(objectType);

            if (underlyingType == null)
                return false;

            var ctors = objectType.GetConstructors();

            return ctors
                .Select(x => x.GetParameters())
                .Any(x => x.Length == 1 && x[0].ParameterType == underlyingType);
        }

        private static Type GetUnderlyingType(Type objectType)
        {
            return objectType
                .GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (IScalarValue<>))
                .Select(x => x.GetGenericArguments()[0])
                .FirstOrDefault();
        }
    }
}
