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
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using iSynaptic.Modeling.Domain;
using Newtonsoft.Json;
using NUnit.Framework;

namespace iSynaptic.Serialization
{
    [TestFixture]
    public class ScalarValueConverterTests
    {
        private static readonly ScalarValueConverter _converter =
            new ScalarValueConverter();

        private static readonly JsonSerializer _serializer =
            JsonSerializer.Create(new JsonSerializerSettings { Converters = new List<JsonConverter> { _converter } });

        private class ScalarValue<T> : IScalarValue<T>
        {
            private readonly T _value;

            public ScalarValue(T value)
            {
                _value = value;
            }

            object IScalarValue.Value
            {
                get { return Value; }
            }

            public T Value { get { return _value; } }
        }

        private class NoConstructorScalarValue<T> : IScalarValue<T>
        {
            object IScalarValue.Value
            {
                get { return Value; }
            }

            public T Value
            {
                get { return default(T); }
            }
        }

        private class Name
        {
            public string First;
            public ScalarValue<string> Last;
        }


        [Test]
        public void CanConvert_WithCorrectlyImplementedScalarValueType_ReturnsTrue()
        {
            _converter.CanConvert(typeof (ScalarValue<int>))
                .Should().BeTrue();

            _converter.CanConvert(typeof (ScalarValue<string>))
                .Should().BeTrue();
        }

        [Test]
        public void CanConvert_WithIncorrectlyImplementedScalarValueType_ReturnsFalse()
        {
            _converter.CanConvert(typeof(NoConstructorScalarValue<int>))
                .Should().BeFalse();

            _converter.CanConvert(typeof(NoConstructorScalarValue<string>))
                .Should().BeFalse();
        }

        [Test]
        public void CanConvert_WithNonScalarValueType_ReturnsFalse()
        {
            _converter.CanConvert(typeof(int))
                .Should().BeFalse();

            _converter.CanConvert(typeof (string))
                .Should().BeFalse();
        }

        [Test]
        public void CanConvert_WithOpenType_ReturnsFalse()
        {
            _converter.CanConvert(typeof(ScalarValue<>))
                .Should().BeFalse();
        }

        [Test]
        public void WriteJson_WritesCorrectly()
        {
            var val = new ScalarValue<int>(42);

            String json = _serializer.Serialize(val);
            json.Should().Be("42");
        }

        [Test]
        public void WriteJson_WithNestedScalars_WritesCorrectly()
        {
            var val = new ScalarValue<ScalarValue<int>>(new ScalarValue<int>(42));

            String json = _serializer.Serialize(val);
            json.Should().Be("42");
        }

        [Test]
        public void ReadJson_ReturnsValue()
        {
            String input = "42";

            ScalarValue<int> val = _serializer.Deserialize<ScalarValue<int>>(input);
            val.Value.Should().Be(42);
        }

        [Test]
        public void ReadJson_WithNestedScalars_ReturnsValue()
        {
            String input = "42";

            ScalarValue<ScalarValue<int>> val = _serializer.Deserialize<ScalarValue<ScalarValue<int>>>(input);
            val.Value.Value.Should().Be(42);
        }

        [Test]
        public void WriteJson_WithComplexValue_WritesCorrectly()
        {
            var n = new Name
            {
                First = "John",
                Last = new ScalarValue<string>("Smith")
            };

            String json = _serializer.Serialize(new ScalarValue<Name>(n));

            json.Should().Be("{\"First\":\"John\",\"Last\":\"Smith\"}");

            var val = _serializer.Deserialize<ScalarValue<Name>>(json);

            val.Value.First.Should().Be("John");
            val.Value.Last.Value.Should().Be("Smith");
        }
    }
}
