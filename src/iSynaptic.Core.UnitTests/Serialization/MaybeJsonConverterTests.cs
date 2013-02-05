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
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Newtonsoft.Json;
using iSynaptic.Commons;

namespace iSynaptic.Serialization
{
    [TestFixture]
    public class MaybeJsonConverterTests
    {
        private static readonly MaybeJsonConverter _converter =
            new MaybeJsonConverter();

        private static readonly JsonSerializer _serializer =
            JsonSerializer.Create(new JsonSerializerSettings {Converters = new List<JsonConverter> {_converter}});

        [Test]
        public void CanConvert_WithMaybeTypes_ReturnsTrue()
        {
            _converter.CanConvert(typeof (Maybe<String>))
                .Should().BeTrue();

            _converter.CanConvert(typeof(Maybe<Int32>))
                .Should().BeTrue();
        }
        
        [Test]
        public void CanConvert_WithOpenMaybeType_ReturnsFalse()
        {
            _converter.CanConvert(typeof(Maybe<>))
                .Should().BeFalse();
        }

        [Test]
        public void WriteJson_WithNoValue_WritesObjectWithHasValueSetToFalse()
        {
            Maybe<String> maybe = Maybe.NoValue;

            String json = _serializer.Serialize(maybe);

            json.Should().Be("{\"hasValue\":false}");
        }

        [Test]
        public void WriteJson_WithValue_WritesObjectWithValue()
        {
            Maybe<String> maybe = "Hello, World!".ToMaybe();

            String json = _serializer.Serialize(maybe);

            json.Should().Be("{\"value\":\"Hello, World!\"}");
        }

        [Test]
        public void ReadJson_WithHasValueFalse_ReturnsNoValue()
        {
            String input = "{\"hasValue\":false}";
            Maybe<String> maybe = _serializer.Deserialize<Maybe<String>>(input);

            maybe.HasValue.Should().BeFalse();
        }

        [Test]
        public void ReadJson_WithValue_ReturnsValue()
        {
            String input = "{\"value\":\"Hello, World!\"}";

            Maybe<String> maybe = _serializer.Deserialize<Maybe<String>>(input);
            maybe.HasValue.Should().BeTrue();
            maybe.Value.Should().Be("Hello, World!");
        }
    }
}
