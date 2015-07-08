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
using iSynaptic.TestDomain;

namespace iSynaptic.Serialization
{
    [TestFixture]
    public class ResultJsonConverterTests
    {
        private static readonly ResultJsonConverter _converter =
            new ResultJsonConverter();
    
        private static readonly JsonSerializer _serializer =
            JsonSerializer.Create(new JsonSerializerSettings { Converters = new List<JsonConverter> { _converter } });


        [Test]
        public void CanConvert_WithResultTypes_ReturnsTrue()
        {
            _converter.CanConvert(typeof(Result<String, String>))
                .Should().BeTrue();

            _converter.CanConvert(typeof(Result<Int32, Int32>))
                .Should().BeTrue();
        }

        [Test]
        public void CanConvert_WithOpenMaybeType_ReturnsFalse()
        {
            _converter.CanConvert(typeof(Result<,>))
                .Should().BeFalse();
        }

        [Test]
        public void WriteJson_SuccessWithNoValueAndNoObservations_WritesCorrectly()
        {
            Result<String, String> result = Result.Success();

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"hasValue\":false,\"wasSuccessful\":true}");
        }

        [Test]
        public void WriteJson_WithUnitValueAndNoUnitObservations_WritesCorrectly()
        {
            Result<Unit, Unit> result = Result.Success();

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"value\":{},\"wasSuccessful\":true}");
        }

        [Test]
        public void WriteJson_WithUnitValueAndOneValueObservations_WritesCorrectly()
        {
            Result<Unit, int> result = Result.Success(42);

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"value\":{},\"wasSuccessful\":true,\"observations\":[42]}");
        }

        [Test]
        public void WriteJson_WithUnitValueAndManyValueObservations_WritesCorrectly()
        {
            Result<Unit, int> result = Result.Success(42, 47, 1138, 1337);

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"value\":{},\"wasSuccessful\":true,\"observations\":[42,47,1138,1337]}");
        }

        [Test]
        public void WriteJson_WithUnitValueAndOneReferenceObservations_WritesCorrectly()
        {
            Result<Unit, string> result = Result.Success("yo!");

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"value\":{},\"wasSuccessful\":true,\"observations\":[\"yo!\"]}");
        }

        [Test]
        public void WriteJson_WithUnitValueAndManyReferenceObservations_WritesCorrectly()
        {
            Result<Unit, string> result = Result.Success("This", "one", "goes", "to", "eleven", "one", "louder");

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"value\":{},\"wasSuccessful\":true,\"observations\":[\"This\",\"one\",\"goes\",\"to\",\"eleven\",\"one\",\"louder\"]}");
        }

        [Test]
        public void WriteJson_WithUnitObservations_WritesCorrectly()
        {
            Result<String, Unit> result = "foo".ToResult();

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"value\":\"foo\",\"wasSuccessful\":true}");
        }

        [Test]
        public void WriteJson_SuccessWithValueAndNoObservations_WritesCorrectly()
        {
            Result<String, String> result = "Hello, World!".ToResult();

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"value\":\"Hello, World!\",\"wasSuccessful\":true}");
        }

        [Test]
        public void WriteJson_SuccessWithNoValueAndObservations_WritesCorrectly()
        {
            Result<String, String> result = Result.Success("It went well!", "Take a break!");

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"hasValue\":false,\"wasSuccessful\":true,\"observations\":[\"It went well!\",\"Take a break!\"]}");
        }

        [Test]
        public void WriteJson_SuccessWithValueAndObservations_WritesCorrectly()
        {
            Result<String, String> result = "Hello, World!".ToResult().ObserveMany("It went well!", "Take a break!");

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"value\":\"Hello, World!\",\"wasSuccessful\":true,\"observations\":[\"It went well!\",\"Take a break!\"]}");
        }

        [Test]
        public void WriteJson_FailureWithNoValueAndNoObservations_WritesCorrectly()
        {
            Result<String, String> result = Result.Failure();

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"hasValue\":false,\"wasSuccessful\":false}");
        }

        [Test]
        public void WriteJson_FailureWithValueAndNoObservations_WritesCorrectly()
        {
            Result<String, String> result = "Hello, World!".ToResult().Fail();

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"value\":\"Hello, World!\",\"wasSuccessful\":false}");
        }

        [Test]
        public void WriteJson_FailureWithNoValueAndObservations_WritesCorrectly()
        {
            Result<String, String> result = Result.Failure("Something bad happened!", "Really bad!");

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"hasValue\":false,\"wasSuccessful\":false,\"observations\":[\"Something bad happened!\",\"Really bad!\"]}");
        }

        [Test]
        public void WriteJson_FailureWithValueAndObservations_WritesCorrectly()
        {
            Result<String, String> result = "Hello, World!".ToResult().Fail().ObserveMany("Something bad happened!", "Really bad!");

            String json = _serializer.Serialize(result);
            json.Should().Be("{\"value\":\"Hello, World!\",\"wasSuccessful\":false,\"observations\":[\"Something bad happened!\",\"Really bad!\"]}");
        }

        [Test]
        public void ReadJson_SuccessWithNoValueAndNoObservations_ReturnsCorrectly()
        {
            String input = "{\"hasValue\":false,\"wasSuccessful\":true}";
            Result<String, String> result = _serializer.Deserialize<Result<String, String>>(input);

            result.HasValue.Should().BeFalse();
            result.WasSuccessful.Should().BeTrue();
            result.Observations.Should().BeEmpty();
        }

        [Test]
        public void ReadJson_SuccessWithValueAndNoObservations_ReturnsCorrectly()
        {
            String input = "{\"value\":\"Hello, World!\",\"wasSuccessful\":true}";
            Result<String, String> result = _serializer.Deserialize<Result<String, String>>(input);

            result.HasValue.Should().BeTrue();
            result.Value.Should().Be("Hello, World!");
            result.WasSuccessful.Should().BeTrue();
            result.Observations.Should().BeEmpty();
        }

        [Test]
        public void ReadJson_SuccessWithNoValueAndObservations_WritesCorrectly()
        {
            String input = "{\"hasValue\":false,\"wasSuccessful\":true,\"observations\":[\"It went well!\",\"Take a break!\"]}";
            Result<String, String> result = _serializer.Deserialize<Result<String, String>>(input);

            result.HasValue.Should().BeFalse();
            result.WasSuccessful.Should().BeTrue();
            result.Observations.Should().Contain("It went well!", "Take a break!");
        }

        [Test]
        public void ReadJson_SuccessWithValueAndObservations_WritesCorrectly()
        {
            String input = "{\"value\":\"Hello, World!\",\"wasSuccessful\":true,\"observations\":[\"It went well!\",\"Take a break!\"]}";
            Result<String, String> result = _serializer.Deserialize<Result<String, String>>(input);

            result.HasValue.Should().BeTrue();
            result.Value.Should().Be("Hello, World!");
            result.WasSuccessful.Should().BeTrue();
            result.Observations.Should().Contain("It went well!", "Take a break!");
        }

        [Test]
        public void ReadJson_FailureWithNoValueAndNoObservations_WritesCorrectly()
        {
            String input = "{\"hasValue\":false,\"wasSuccessful\":false}";
            Result<String, String> result = _serializer.Deserialize<Result<String, String>>(input);

            result.HasValue.Should().BeFalse();
            result.WasSuccessful.Should().BeFalse();
            result.Observations.Should().BeEmpty();
        }

        [Test]
        public void ReadJson_FailureWithValueAndNoObservations_WritesCorrectly()
        {
            String input = "{\"value\":\"Hello, World!\",\"wasSuccessful\":false}";
            Result<String, String> result = _serializer.Deserialize<Result<String, String>>(input);

            result.HasValue.Should().BeTrue();
            result.Value.Should().Be("Hello, World!");
            result.WasSuccessful.Should().BeFalse();
            result.Observations.Should().BeEmpty();
        }

        [Test]
        public void ReadJson_FailureWithNoValueAndObservations_WritesCorrectly()
        {
            String input = "{\"hasValue\":false,\"wasSuccessful\":false,\"observations\":[\"Something bad happened!\",\"Really bad!\"]}";
            Result<String, String> result = _serializer.Deserialize<Result<String, String>>(input);

            result.HasValue.Should().BeFalse();
            result.WasSuccessful.Should().BeFalse();
            result.Observations.Should().Contain("Something bad happened!", "Really bad!");
        }

        [Test]
        public void ReadJson_FailureWithValueAndObservations_WritesCorrectly()
        {
            String input = "{\"value\":\"Hello, World!\",\"wasSuccessful\":false,\"observations\":[\"Something bad happened!\",\"Really bad!\"]}";
            Result<String, String> result = _serializer.Deserialize<Result<String, String>>(input);

            result.HasValue.Should().BeTrue();
            result.Value.Should().Be("Hello, World!");
            result.WasSuccessful.Should().BeFalse();
            result.Observations.Should().Contain("Something bad happened!", "Really bad!");
        }

        [Test]
        public void ReadJson_ValueTypeIsNeededToDeserialize_ReadsCorrectly()
        {
            var id = Guid.NewGuid();
            var input = "{\"value\":{\"$type\":\"ServiceCaseId\",\"Value\":\"" + id + "\"},\"wasSuccessful\":true}";

            var result = _serializer.Deserialize<Result<ServiceCaseId, string>>(input);

            result.Value.Should().BeOfType<ServiceCaseId>();
            result.Cast<ServiceCaseId>().Value.Should().Be(new ServiceCaseId(id));
            result.Observations.Should().BeEmpty();
        }
    }
}
