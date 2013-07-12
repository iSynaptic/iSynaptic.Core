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
using System.Text;
using System.Threading.Tasks;

using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using Newtonsoft.Json;
using iSynaptic.Commons;
using iSynaptic.Commons.Collections.Generic;
using iSynaptic.Commons.Linq;
using iSynaptic.Modeling;
using iSynaptic.Modeling.Domain;
using iSynaptic.Serialization;

namespace iSynaptic.Core.Persistence
{
    public class EventStoreAggregateRepository<TAggregate, TIdentifier> : AggregateRepository<TAggregate, TIdentifier>
        where TAggregate : class, IAggregate<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly LogicalTypeRegistry _logicalTypeRegistry;

        private readonly JsonSerializer _dataSerializer;
        private readonly JsonSerializer _metadataSerializer;

        private readonly Func<EventStoreConnection> _connectionFactory;

        public EventStoreAggregateRepository(LogicalTypeRegistry logicalTypeRegistry, Func<EventStoreConnection> connectionFactory)
        {
            _logicalTypeRegistry = Guard.NotNull(logicalTypeRegistry, "logicalTypeRegistry");
            _connectionFactory = Guard.NotNull(connectionFactory, "connectionFactory");

            _dataSerializer = JsonSerializerBuilder.Build(logicalTypeRegistry);

            var metadataSerializerSettings = JsonSerializerBuilder.BuildSettings(logicalTypeRegistry);
            metadataSerializerSettings.TypeNameHandling = TypeNameHandling.None;

            _metadataSerializer = JsonSerializer.Create(metadataSerializerSettings);
        }

        protected override async Task<AggregateSnapshotLoadFrame<TIdentifier>> GetSnapshot(TIdentifier id, int maxVersion)
        {
            using (var cn = _connectionFactory())
            {
                String snapshotStreamId = BuildSnapshotStreamIdentifier(id);

                var resolvedEvent = (await cn.ReadStreamEventsForwardAsync(snapshotStreamId, 1, int.MaxValue, false))
                    .ToMaybe()
                    .Where(x => x.Status == SliceReadStatus.Success)
                    .SelectMany(x => x.Events)
                    .TrySingle();

                var snapshot = resolvedEvent
                    .Select(x => x.Event.Data)
                    .Select(Encoding.Default.GetString)
                    .Select(x => _dataSerializer.Deserialize<IAggregateSnapshot<TIdentifier>>(x))
                    .Where(x => x.Version <= maxVersion);

                var metadata = resolvedEvent
                    .Select(x => x.Event.Metadata)
                    .Select(Encoding.Default.GetString)
                    .Select(x => _metadataSerializer.Deserialize<Dictionary<String, String>>(x));

                if (snapshot.HasValue && metadata.HasValue)
                {
                    Type aggregateType = _logicalTypeRegistry.LookupActualType(
                        LogicalType.Parse(metadata.Value["aggregateType"]));

                    return new AggregateSnapshotLoadFrame<TIdentifier>(aggregateType, id, snapshot.Value);
                }

                return null;
            }
        }

        protected override async Task<AggregateEventsLoadFrame<TIdentifier>> GetEvents(TIdentifier id, int minVersion, int maxVersion)
        {
            var maxCount = (maxVersion - minVersion) + 1;

            if (maxCount <= 0)
                return null;

            using (var cn = _connectionFactory())
            {
                String streamId = BuildStreamIdentifier(id);

                var resolvedEvents = (await cn.ReadStreamEventsForwardAsync(streamId, minVersion, maxCount, false))
                    .ToMaybe()
                    .Where(x => x.Status == SliceReadStatus.Success)
                    .SelectMany(x => x.Events)
                    .ToArray();

                var metadata = resolvedEvents.TryFirst()
                    .Select(x => x.Event.Metadata)
                    .Select(Encoding.Default.GetString)
                    .Select(x => _metadataSerializer.Deserialize<Dictionary<String, String>>(x));

                if (resolvedEvents.Length > 0 && metadata.HasValue)
                {
                    Type aggregateType = _logicalTypeRegistry.LookupActualType(
                        LogicalType.Parse(metadata.Value["aggregateType"]));

                    var events = resolvedEvents
                        .Select(x => x.Event.Data)
                        .Select(Encoding.Default.GetString)
                        .Select(x => _dataSerializer.Deserialize<IAggregateEvent<TIdentifier>>(x));

                    return new AggregateEventsLoadFrame<TIdentifier>(aggregateType, id, events);
                }

                return null;
            }
        }

        protected async override Task SaveSnapshot(AggregateSnapshotSaveFrame<TIdentifier> frame)
        {
            using (var cn = _connectionFactory())
            {
                var aggregateType = frame.AggregateType;
                var id = frame.Id;
                var snapshot = frame.Snapshot;

                String streamId = BuildSnapshotStreamIdentifier(id);

                var metadata = BuildSnapshotMetadata(aggregateType);

                var stream = await cn.ReadStreamEventsForwardAsync(streamId, 0, 1, false);
                if (stream.Status == SliceReadStatus.StreamNotFound)
                {
                    await cn.CreateStreamAsync(
                        streamId,
                        snapshot.SnapshotId,
                        true,
                        Encoding.Default.GetBytes(_metadataSerializer.Serialize(metadata)));

                    await cn.AppendToStreamAsync(
                        streamId,
                        ExpectedVersion.EmptyStream,
                        BuildEventData(
                            snapshot.SnapshotId,
                            snapshot,
                            aggregateType,
                            metadata
                            )
                        );
                }
            }
        }

        private Dictionary<String, Object> BuildSnapshotMetadata(Type aggregateType)
        {
            return BuildMetadata(
                KeyValuePair.Create<String, Object>(
                    "aggregateType",
                    _logicalTypeRegistry.LookupLogicalType(aggregateType).ToString()),
                KeyValuePair.Create<String, Object>(
                    "$maxCount",
                    1)
                );
        }

        protected async override Task SaveEvents(AggregateEventsSaveFrame<TIdentifier> frame)
        {
            var aggregateType = frame.AggregateType;
            var id = frame.Id;
            var evts = frame.Events.ToArray();

            if(evts.Length <= 0)
                throw new ArgumentException("There are no events to save.", "frame");

            String streamId = BuildStreamIdentifier(id);
            using (var cn = _connectionFactory())
            {
                int expectedVersion = evts[0].Version == 1
                    ? ExpectedVersion.NoStream
                    : evts[0].Version - 1;

                var metadata = BuildMetadata(
                    KeyValuePair.Create<String, Object>(
                        "aggregateType",
                        _logicalTypeRegistry.LookupLogicalType(aggregateType).ToString()
                        )
                    );

                try
                {
                    await cn.AppendToStreamAsync(
                        streamId,
                        expectedVersion,
                        evts.Select(e => BuildEventData(
                                e.EventId,
                                e,
                                aggregateType, 
                                metadata
                            )));

                }
                catch (WrongExpectedVersionException ex)
                {
                    throw new AggregateConcurrencyException(ex);
                }
            }
        }

        protected Dictionary<String, Object> BuildMetadata(params KeyValuePair<String, Object>[] metadata)
        {
            return metadata.ToDictionary();
        }

        protected EventData BuildEventData(Guid id, object data, Type aggregateType, Dictionary<String, Object> metadata)
        {
            return new EventData(id,
                _logicalTypeRegistry.LookupLogicalType(data.GetType()).ToString(),
                true,
                Encoding.Default.GetBytes(_dataSerializer.Serialize(data)),
                Encoding.Default.GetBytes(_metadataSerializer.Serialize(metadata)));
        }

        protected virtual String BuildStreamIdentifier(TIdentifier id)
        {
            return String.Format("{0}-{1}", _logicalTypeRegistry.LookupLogicalType(id.GetType()), id);
        }

        protected virtual String BuildSnapshotStreamIdentifier(TIdentifier id)
        {
            return String.Format("{0}-snapshot", BuildStreamIdentifier(id));
        }
    }
}
