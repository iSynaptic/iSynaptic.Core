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
        private static readonly Guid _offsetEventId = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
        private static readonly EventData _offsetEvent = new EventData(_offsetEventId, "streamOffset", true, Encoding.Default.GetBytes("{}"), null);

        private readonly ILogicalTypeRegistry _logicalTypeRegistry;

        private readonly JsonSerializer _dataSerializer;
        private readonly JsonSerializer _metadataSerializer;

        private readonly Func<IEventStoreConnection> _connectionFactory;

        public EventStoreAggregateRepository(ILogicalTypeRegistry logicalTypeRegistry, Func<IEventStoreConnection> connectionFactory)
        {
            _logicalTypeRegistry = Guard.NotNull(logicalTypeRegistry, "logicalTypeRegistry");
            _connectionFactory = Guard.NotNull(connectionFactory, "connectionFactory");

            _dataSerializer = JsonSerializerBuilder.Build(logicalTypeRegistry);

            var metadataSerializerSettings = JsonSerializerBuilder.BuildSettings(logicalTypeRegistry);
            metadataSerializerSettings.TypeNameHandling = TypeNameHandling.None;

            _metadataSerializer = JsonSerializer.Create(metadataSerializerSettings);
        }

        protected override async Task<AggregateEventsLoadFrame<TIdentifier>> GetEvents(TIdentifier id, int minVersion, int maxVersion)
        {
            var maxCount = (maxVersion - minVersion) + 1;

            if (maxCount <= 0)
                return null;

            using (var cn = _connectionFactory())
            {
                await cn.ConnectAsync().ConfigureAwait(false);

                String streamId = BuildStreamIdentifier(id);

                var metadataResult = await cn.GetStreamMetadataAsync(streamId).ConfigureAwait(false);
                if (metadataResult.MetastreamVersion == ExpectedVersion.NoStream)
                    return null;

                string aggregateTypeString;

                if (!metadataResult.StreamMetadata.TryGetValue("aggregateType", out aggregateTypeString))
                {
                    throw new InvalidOperationException("Aggregate type is not specified in event stream metadata.");
                }

                var resolvedEvents = (await cn.ReadStreamEventsForwardAsync(streamId, minVersion, maxCount, false).ConfigureAwait(false))
                    .ToMaybe()
                    .Where(x => x.Status == SliceReadStatus.Success)
                    .SelectMany(x => x.Events)
                    .ToArray();

                if (resolvedEvents.Length > 0)
                {
                    Type aggregateType = _logicalTypeRegistry.LookupActualType(LogicalType.Parse(aggregateTypeString));

                    var events = resolvedEvents
                        .Select(x => x.Event.Data)
                        .Select(Encoding.Default.GetString)
                        .Select(x => _dataSerializer.Deserialize<IAggregateEvent<TIdentifier>>(x));

                    return new AggregateEventsLoadFrame<TIdentifier>(aggregateType, id, events);
                }

                return null;
            }
        }

        protected async override Task SaveEvents(AggregateEventsSaveFrame<TIdentifier> frame)
        {
            var aggregateType = frame.AggregateType;
            var id = frame.Id;
            var evts = frame.Events.ToArray();

            if (evts.Length <= 0)
                throw new ArgumentException("There are no events to save.", "frame");

            String streamId = BuildStreamIdentifier(id);
            using (var cn = _connectionFactory())
            {
                await cn.ConnectAsync().ConfigureAwait(false);

                bool isNewStream = evts[0].Version == 1;

                try
                {
                    var events = evts.Select(e => BuildEventData(e.EventId, e, aggregateType));

                    if (isNewStream)
                    {
                        var metadata = BuildStreamMetadata(aggregateType);
                        await cn.SetStreamMetadataAsync(streamId, ExpectedVersion.NoStream, metadata).ConfigureAwait(false);

                        events = new[] { _offsetEvent }.Concat(events);
                    }

                    int expectedVersion = isNewStream
                        ? ExpectedVersion.EmptyStream
                        : evts[0].Version - 1;

                    await cn.AppendToStreamAsync(
                        streamId,
                        expectedVersion,
                        events).ConfigureAwait(false);

                }
                catch (WrongExpectedVersionException ex)
                {
                    throw new AggregateConcurrencyException(ex);
                }
            }
        }

        protected override async Task<AggregateSnapshotLoadFrame<TIdentifier>> GetSnapshot(TIdentifier id, int maxVersion)
        {
            using (var cn = _connectionFactory())
            {
                await cn.ConnectAsync().ConfigureAwait(false);

                String snapshotStreamId = BuildSnapshotStreamIdentifier(id);

                var metadataResult = await cn.GetStreamMetadataAsync(snapshotStreamId).ConfigureAwait(false);
                if (metadataResult.MetastreamVersion == ExpectedVersion.NoStream)
                    return null;

                string aggregateTypeString;

                if (!metadataResult.StreamMetadata.TryGetValue("aggregateType", out aggregateTypeString))
                {
                    throw new InvalidOperationException("Aggregate type is not specified in event stream metadata.");
                }
                
                var resolvedEvent = (await cn.ReadStreamEventsForwardAsync(snapshotStreamId, 0, int.MaxValue, false).ConfigureAwait(false))
                    .ToMaybe()
                    .Where(x => x.Status == SliceReadStatus.Success)
                    .SelectMany(x => x.Events)
                    .TrySingle();

                var snapshot = resolvedEvent
                    .Select(x => x.Event.Data)
                    .Select(Encoding.Default.GetString)
                    .Select(x => _dataSerializer.Deserialize<IAggregateSnapshot<TIdentifier>>(x))
                    .Where(x => x.Version <= maxVersion);

                if (snapshot.HasValue)
                {
                    Type aggregateType = _logicalTypeRegistry.LookupActualType(LogicalType.Parse(aggregateTypeString));

                    return new AggregateSnapshotLoadFrame<TIdentifier>(aggregateType, id, snapshot.Value);
                }

                return null;
            }
        }

        protected async override Task SaveSnapshot(AggregateSnapshotSaveFrame<TIdentifier> frame)
        {
            using (var cn = _connectionFactory())
            {
                await cn.ConnectAsync().ConfigureAwait(false);

                var aggregateType = frame.AggregateType;
                var id = frame.Id;
                var snapshot = frame.Snapshot;

                String streamId = BuildSnapshotStreamIdentifier(id);

                var metadata = BuildSnapshotStreamMetadata(aggregateType);

                var stream = await cn.ReadStreamEventsForwardAsync(streamId, 0, int.MaxValue, false).ConfigureAwait(false);
                if (stream.Status == SliceReadStatus.StreamNotFound)
                {
                    await cn.SetStreamMetadataAsync(streamId, ExpectedVersion.NoStream, metadata).ConfigureAwait(false);

                    await cn.AppendToStreamAsync(
                        streamId,
                        ExpectedVersion.EmptyStream,
                        BuildEventData(
                            snapshot.SnapshotId,
                            snapshot,
                            aggregateType)
                        ).ConfigureAwait(false);
                }
                else
                {
                    await cn.AppendToStreamAsync(
                        streamId,
                        stream.LastEventNumber,
                        BuildEventData(
                            snapshot.SnapshotId,
                            snapshot,
                            aggregateType)
                    ).ConfigureAwait(false);    
                }
            }
        }

        private StreamMetadata BuildSnapshotStreamMetadata(Type aggregateType)
        {
            return StreamMetadata.Build()
                .SetMaxCount(1)
                .SetCustomProperty("aggregateType", _logicalTypeRegistry.LookupLogicalType(aggregateType).ToString())
                .Build();
        }

        private StreamMetadata BuildStreamMetadata(Type aggregateType)
        {
            return StreamMetadata.Build()
                .SetCustomProperty("aggregateType", _logicalTypeRegistry.LookupLogicalType(aggregateType).ToString())
                .Build();
        }

        protected EventData BuildEventData(Guid id, object data, Type aggregateType)
        {
            return new EventData(id,
                _logicalTypeRegistry.LookupLogicalType(data.GetType()).ToString(),
                true,
                Encoding.Default.GetBytes(_dataSerializer.Serialize(data)),
                null);
        }

        protected virtual String BuildStreamIdentifier(TIdentifier id)
        {
            return _dataSerializer.Serialize(id);
        }

        protected virtual String BuildSnapshotStreamIdentifier(TIdentifier id)
        {
            return String.Format("{0}-snapshot", BuildStreamIdentifier(id));
        }
    }
}
