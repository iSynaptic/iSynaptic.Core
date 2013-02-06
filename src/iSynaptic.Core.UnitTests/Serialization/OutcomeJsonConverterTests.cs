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
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Newtonsoft.Json;
using iSynaptic.Commons;

namespace iSynaptic.Serialization
{
    [TestFixture]
    public class OutcomeJsonConverterTests
    {
        private static readonly OutcomeJsonConverter _converter =
            new OutcomeJsonConverter();

        private static readonly JsonSerializer _serializer =
            JsonSerializer.Create(new JsonSerializerSettings {Converters = new List<JsonConverter> {_converter}});

        [Test]
        public void CanConvert_WithOutcomeTypes_ReturnsTrue()
        {
            _converter.CanConvert(typeof(Outcome<String>))
                .Should().BeTrue();

            _converter.CanConvert(typeof(Outcome<Int32>))
                .Should().BeTrue();
        }

        [Test]
        public void CanConvert_WithOpenOutcomeType_ReturnsFalse()
        {
            _converter.CanConvert(typeof(Outcome<>))
                .Should().BeFalse();
        }

        [Test]
        public void WriteJson_SuccessWithNoObservations_WritesCorrectly()
        {
            Outcome<String> outcome = Outcome.Success();

            String json = _serializer.Serialize(outcome);

            json.Should().Be("{\"wasSuccessful\":true}");
        }

        [Test]
        public void WriteJson_FailureWithNoObservations_WritesCorrectly()
        {
            Outcome<String> outcome = Outcome.Failure();

            String json = _serializer.Serialize(outcome);

            json.Should().Be("{\"wasSuccessful\":false}");
        }

        [Test]
        public void WriteJson_SuccessWithObservations_WritesCorrectly()
        {
            Outcome<String> outcome = Outcome.Success("All is well!", "Go take a break!");

            String json = _serializer.Serialize(outcome);

            json.Should().Be("{\"wasSuccessful\":true,\"observations\":[\"All is well!\",\"Go take a break!\"]}");
        }

        [Test]
        public void WriteJson_FailureWithObservations_WritesCorrectly()
        {
            Outcome<String> outcome = Outcome.Failure("Something very bad happened!", "Really, really bad!");

            String json = _serializer.Serialize(outcome);

            json.Should().Be("{\"wasSuccessful\":false,\"observations\":[\"Something very bad happened!\",\"Really, really bad!\"]}");
        }

        [Test]
        public void ReadJson_SuccessWithNoObservations_ReturnsSuccessOutcome()
        {
            String input = "{\"wasSuccessful\":true}";
            Outcome<String> outcome = _serializer.Deserialize<Outcome<String>>(input);

            outcome.WasSuccessful.Should().BeTrue();
            outcome.Observations.Should().BeEmpty();
        }

        [Test]
        public void ReadJson_FailureWithNoObservations_ReturnsFailureOutcome()
        {
            String input = "{\"wasSuccessful\":false}";
            Outcome<String> outcome = _serializer.Deserialize<Outcome<String>>(input);

            outcome.WasSuccessful.Should().BeFalse();
            outcome.Observations.Should().BeEmpty();
        }

        [Test]
        public void ReadJson_SuccessWithObservations_ReturnsSuccessOutcome()
        {
            String input = "{\"wasSuccessful\":true,\"observations\":[\"All is well!\",\"Go take a break!\"]}";
            Outcome<String> outcome = _serializer.Deserialize<Outcome<String>>(input);

            outcome.WasSuccessful.Should().BeTrue();
            outcome.Observations.Should().Contain("All is well!", "Go take a break!");
        }

        [Test]
        public void ReadJson_FailureWithObservations_ReturnsFailureOutcome()
        {
            String input = "{\"wasSuccessful\":false,\"observations\":[\"Something very bad happened!\",\"Really, really bad!\"]}";
            Outcome<String> outcome = _serializer.Deserialize<Outcome<String>>(input);

            outcome.WasSuccessful.Should().BeFalse();
            outcome.Observations.Should().Contain("Something very bad happened!", "Really, really bad!");
        }
    }
}
