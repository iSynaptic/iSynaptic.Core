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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using iSynaptic.Commons;
using iSynaptic.Commons.Collections.Generic;
using iSynaptic.Commons.Linq;

namespace iSynaptic
{
    public class InMemoryJsonAggregateRepository<TAggregate, TIdentifier> : AggregateRepository<TAggregate, TIdentifier>
        where TAggregate : class, IAggregate<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly Dictionary<TIdentifier, String> _state =
            new Dictionary<TIdentifier, String>();

        private readonly Task _completedTask;

        private readonly JsonSerializer _serializer =
            JsonSerializer.Create(new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = new SmartContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new MaybeJsonConverter()
                }
            });

        public InMemoryJsonAggregateRepository()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            _completedTask = tcs.Task;
        }

        public override Task<AggregateMemento<TIdentifier>> GetMemento(TIdentifier id, int maxVersion)
        {
            lock (_state)
            {
                return Task.FromResult(_state.TryGetValue(id)
                      .Select(Deserialize)
                      .Select(x =>
                      {
                          var s = x.Snapshot.Where(y => y.Version <= maxVersion);

                          return new AggregateMemento<TIdentifier>(
                              x.AggregateType,
                              s,
                              x.Events.SkipWhile(y => y.Version <= s.Select(z => z.Version).ValueOrDefault())
                               .TakeWhile(y => y.Version <= maxVersion));
                      })
                      .ValueOrDefault());
            }
        }

        protected override Task SaveSnapshot(Type aggregateType, IAggregateSnapshot<TIdentifier> snapshot)
        {
            lock (_state)
            {
                var state = _state.TryGetValue(snapshot.Id)
                                .Select(Deserialize)
                                .ValueOrDefault();

                if (state != null && state.Snapshot.Where(x => x.Version >= snapshot.Version).HasValue)
                    throw new InvalidOperationException("Snapshot already exists that covers this aggregate version.");

                _state[snapshot.Id] = Serialize(new AggregateMemento<TIdentifier>(aggregateType, snapshot.ToMaybe(), state != null ? state.Events : null));
            }

            return _completedTask;
        }

        protected override Task SaveEventStream(Type aggregateType, TIdentifier id, IEnumerable<IAggregateEvent<TIdentifier>> events)
        {
            lock (_state)
            {
                _state[id] = _state.TryGetValue(id)
                    .Select(x => _serializer.Deserialize<AggregateMemento<TIdentifier>>(new JsonTextReader(new StringReader(x))))
                    .Select(x =>
                    {
                        var lastEvent = x.Events.TryLast();
                        return Serialize(new AggregateMemento<TIdentifier>(
                            aggregateType,
                            x.Snapshot,
                            events.SkipWhile(y => y.Version <= lastEvent.Select(z => z.Version).ValueOrDefault())));
                    })
                    .ValueOrDefault(() => Serialize(new AggregateMemento<TIdentifier>(aggregateType, Maybe<IAggregateSnapshot<TIdentifier>>.NoValue, events)));
            }

            return _completedTask;
        }

        private String Serialize(AggregateMemento<TIdentifier> memento)
        {
            var writer = new StringWriter();
            _serializer.Serialize(writer, memento);

            return writer.GetStringBuilder().ToString();
        }

        private AggregateMemento<TIdentifier> Deserialize(String json)
        {
            return _serializer.Deserialize<AggregateMemento<TIdentifier>>(new JsonTextReader(new StringReader(json)));
        }

    }

    internal class SmartContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
    }

    internal class MaybeJsonConverter : JsonConverter
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
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == _maybeTypeDefinition;
        }
    }
}
