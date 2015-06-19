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
using Newtonsoft.Json.Serialization;

namespace iSynaptic.Serialization
{
    [TestFixture]
    public class MaybeJsonConverterTests
    {
        [SetUp]
        public void OnSetUp()
        {
            Settings = GetDefaultSerializerSettings();
        }

        private JsonSerializerSettings GetDefaultSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new MaybeJsonConverter(), new OutcomeJsonConverter() },
                TypeNameHandling = TypeNameHandling.Auto,

                // HACK: Reference loop detection is using value comparison instead of
                //       reference comparison. Remove when this is fixed
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
        }

        public JsonSerializerSettings Settings
        {
            get; set;
        }

        public JsonSerializer Serializer
        {
            get
            {
                return JsonSerializer.Create(Settings);
            }
        }


        [Test]
        public void CanConvert_WithMaybeTypes_ReturnsTrue()
        {
            new MaybeJsonConverter().CanConvert(typeof(Maybe<String>))
                .Should().BeTrue();

            new MaybeJsonConverter().CanConvert(typeof(Maybe<Int32>))
                .Should().BeTrue();
        }

        [Test]
        public void CanConvert_WithOpenMaybeType_ReturnsFalse()
        {
            new MaybeJsonConverter().CanConvert(typeof(Maybe<>))
                .Should().BeFalse();
        }

        [Test]
        public void WriteJson_WithNoValue_WritesCorrectly()
        {
            Maybe<String> maybe = Maybe.NoValue;

            String json = Serializer.Serialize(maybe);

            json.Should().Be("{\"hasValue\":false}");
        }

        [Test]
        public void WriteJson_WithValue_WritesCorrectly()
        {
            Maybe<String> maybe = "Hello, World!".ToMaybe();

            String json = Serializer.Serialize(maybe);

            json.Should().Be("{\"value\":\"Hello, World!\"}");
        }

        [Test]
        public void ReadJson_WithHasValueFalse_ReturnsNoValue()
        {
            String input = "{\"hasValue\":false}";
            Maybe<String> maybe = Serializer.Deserialize<Maybe<String>>(input);

            maybe.HasValue.Should().BeFalse();
        }

        [Test]
        public void ReadJson_WithValue_ReturnsValue()
        {
            String input = "{\"value\":\"Hello, World!\"}";

            Maybe<String> maybe = Serializer.Deserialize<Maybe<String>>(input);
            maybe.HasValue.Should().BeTrue();
            maybe.Value.Should().Be("Hello, World!");
        }

        [Test]
        public void EmptyMaybeOfOutcome_CanSerialize()
        {
            Maybe<Outcome<string>> maybe = Maybe.NoValue;

            String json = Serializer.Serialize(maybe);

            json.Should().Be("{\"hasValue\":false}");
        }

        [Test]
        public void ReadJson_MaybeOfOutcome_WithSuccessOutcomeNoObservations_CanDeserialize()
        {
            String json = "{\"value\":{\"wasSuccessful\":true}}";

            var maybe = Serializer.Deserialize<Maybe<Outcome<Unit>>>(json);
            maybe.HasValue.Should().BeTrue();
            maybe.Value.WasSuccessful.Should().BeTrue();
            maybe.Value.Observations.Should().BeEmpty();
        }


        [Test]
        public void ReadJson_MaybeOfOutcome_WithSuccessOutcomeOneValueObservations_CanDeserialize()
        {
            String json = "{\"value\":{\"wasSuccessful\":true,\"observations\":[42]}}";
            var maybe = Serializer.Deserialize<Maybe<Outcome<int>>>(json);

            maybe.HasValue.Should().BeTrue();
            maybe.Value.WasSuccessful.Should().BeTrue();
            maybe.Value.Observations.Should().BeEquivalentTo(42);
        }

        [Test]
        public void ReadJson_MaybeOfOutcome_WithSuccessOutcomeManyValueObservations_CanDeserialize()
        {
            String json = "{\"value\":{\"wasSuccessful\":true,\"observations\":[42,47,1138,1337]}}";
            var maybe = Serializer.Deserialize<Maybe<Outcome<int>>>(json);

            maybe.HasValue.Should().BeTrue();
            maybe.Value.WasSuccessful.Should().BeTrue();
            maybe.Value.Observations.Should().BeEquivalentTo(42, 47, 1138, 1337);
        }

        [Test]
        public void ReadJson_MaybeOfOutcome_WithSuccessOutcomeOneReferenceObservations_CanDeserialize()
        {
            String json = "{\"value\":{\"wasSuccessful\":true,\"observations\":[\"yo!\"]}}";
            var maybe = Serializer.Deserialize<Maybe<Outcome<string>>>(json);

            maybe.HasValue.Should().BeTrue();
            maybe.Value.WasSuccessful.Should().BeTrue();
            maybe.Value.Observations.Should().BeEquivalentTo("yo!");
        }

        [Test]
        public void ReadJson_MaybeOfOutcome_WithSuccessOutcomeManyReferenceObservations_CanDeserialize()
        {
            String json = "{\"value\":{\"wasSuccessful\":true,\"observations\":[\"This\",\"one\",\"goes\",\"to\",\"eleven\",\"one\",\"louder\"]}}";
            var maybe = Serializer.Deserialize<Maybe<Outcome<string>>>(json);

            maybe.HasValue.Should().BeTrue();
            maybe.Value.WasSuccessful.Should().BeTrue();
            maybe.Value.Observations.Should().BeEquivalentTo("This", "one", "goes", "to", "eleven", "one", "louder");
        }

        [Test]
        public void WriteJson_MaybeOfOutcome_WithSuccessOutcomeNoObservations_CanSerialize()
        {
            var maybe = Outcome.Success().ToMaybe();

            String json = Serializer.Serialize(maybe);

            json.Should().Be("{\"value\":{\"wasSuccessful\":true}}");
        }

        [Test]
        public void WriteJson_MaybeOfOutcome_WithSuccessOutcomeOneValueObservations_CanSerialize()
        {
            var maybe = Outcome.Success(42).ToMaybe();

            String json = Serializer.Serialize(maybe);

            json.Should().Be("{\"value\":{\"wasSuccessful\":true,\"observations\":[42]}}");
        }

        [Test]
        public void WriteJson_MaybeOfOutcome_WithSuccessOutcomeManyValueObservations_CanSerialize()
        {
            var maybe = Outcome.Success(42, 47, 1138, 1337).ToMaybe();

            String json = Serializer.Serialize(maybe);

            json.Should().Be("{\"value\":{\"wasSuccessful\":true,\"observations\":[42,47,1138,1337]}}");
        }

        [Test]
        public void WriteJson_MaybeOfOutcome_WithSuccessOutcomeOneReferenceObservations_CanSerialize()
        {
            var maybe = Outcome.Success("yo!").ToMaybe();

            String json = Serializer.Serialize(maybe);

            json.Should().Be("{\"value\":{\"wasSuccessful\":true,\"observations\":[\"yo!\"]}}");
        }

        [Test]
        public void WriteJson_MaybeOfOutcome_WithSuccessOutcomeManyReferenceObservations_CanSerialize()
        {
            var maybe = Outcome.Success("This", "one", "goes", "to", "eleven", "one", "louder").ToMaybe();

            String json = Serializer.Serialize(maybe);

            json.Should().Be("{\"value\":{\"wasSuccessful\":true,\"observations\":[\"This\",\"one\",\"goes\",\"to\",\"eleven\",\"one\",\"louder\"]}}");
        }

        [Test]
        public void WriteJson_MaybeOfValue_InCovariantObjectProperty_CanSerialize()
        {
            var subject = new ClassWithObjectProperty { Value = 42.ToMaybe() };

            String json = Serializer.Serialize(subject);

            var expected = "{\"Value\":{\"value\":42}}";
            json.Should().Be(expected);
        }
    }
}
