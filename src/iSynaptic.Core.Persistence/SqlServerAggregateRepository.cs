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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using iSynaptic.Commons;
using iSynaptic.Modeling.Domain;
using iSynaptic.Serialization;

namespace iSynaptic.Core.Persistence
{
    public class SqlServerAggregateRepository : AggregateRepository
    {
        private static readonly Regex _scriptRegex = new Regex(@"(?<script>.+?)(\r\nGO(\r\n|$))", 
            RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

        public static async Task EnsureTablesExist(string connectionString)
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("iSynaptic.Core.Persistence.AggregateStore.sql"))
            using (var reader = new StreamReader(stream))
            {
                string script = await reader.ReadToEndAsync();
                var matches = _scriptRegex.Matches(script);

                using (var connection = new SqlConnection(connectionString))
                {
                    var commands = matches.OfType<Match>()
                        .Select(x => x.Groups["script"].Value)
                        .Select(sql => new SqlCommand
                        {
                            Connection = connection,
                            CommandType = CommandType.Text,
                            CommandText = sql
                        });

                    await connection.OpenAsync();

                    foreach(var command in commands)
                        await command.ExecuteNonQueryAsync();
                }
            }
        }

        #region Helper Classes

        private abstract class DbRow
        {
            private string _aggregateId;
            private int _version;
            private string _type;

            protected DbRow(string aggregateId, int version, string type)
            {
                _aggregateId = ValidateAggregateId(aggregateId, "aggregateId");
                _version = ValidateVersion(version, "version");
                _type = ValidateType(type, "type");
            }

            private string ValidateAggregateId(string value, string argumentName)
            {
                if (value == null) throw new ArgumentNullException(argumentName);
                if (value.Length > 200) throw new ArgumentException("Aggregate ID must be 200 characters or less.", argumentName);

                return value;
            }

            private int ValidateVersion(int value, string argumentName)
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(argumentName, "Version must be greater than or equal to one (1).");

                return value;
            }

            private string ValidateType(string value, string argumentName)
            {
                if (value == null) throw new ArgumentNullException(argumentName);
                if (value.Length > 200) throw new ArgumentException("Type must be 200 characters or less.", argumentName);

                return value;
            }

            public string AggregateId
            {
                get { return _aggregateId; }
                set { _aggregateId = ValidateAggregateId(value, "value"); }
            }

            public int Version
            {
                get { return _version; }
                set { _version = ValidateVersion(value, "value"); }
            }

            public string Type
            {
                get { return _type; }
                set { _type = ValidateType(value, "value"); }
            }
        }

        private abstract class DbDataRow : DbRow
        {
            private string _data;

            protected DbDataRow(string aggregateId, int version, string type, string data)
                : base(aggregateId, version, type)
            {
                _data = ValidateData(data, "data");
            }

            private string ValidateData(string data, string argumentName)
            {
                if (data == null) throw new ArgumentNullException(argumentName);
                if (string.IsNullOrWhiteSpace(data)) throw new ArgumentException("Data cannot be only whitespace.", argumentName);

                return data;
            }

            public string Data
            {
                get { return _data; }
                set { _data = ValidateData(value, "value"); }
            }
        }

        private class DbAggregate : DbRow
        {
            public DbAggregate(string aggregateId, int version, string type)
                : base(aggregateId, version, type)
            {
            }
        }

        private class DbAggregateEvent : DbDataRow
        {
            public DbAggregateEvent(string aggregateId, int version, string type, string data)
                : base(aggregateId, version, type, data)
            {
            }
        }

        private class DbAggregateSnapshot : DbDataRow
        {
            public DbAggregateSnapshot(string aggregateId, int version, string type, string data)
                : base(aggregateId, version, type, data)
            {
            }
        }

        #endregion

        private readonly ILogicalTypeRegistry _logicalTypeRegistry;
        private readonly JsonSerializer _dataSerializer;

        private readonly string _connectionString;

        public SqlServerAggregateRepository(ILogicalTypeRegistry logicalTypeRegistry, string connectionString)
        {
            _logicalTypeRegistry = Guard.NotNull(logicalTypeRegistry, "logicalTypeRegistry");
            _dataSerializer = JsonSerializerBuilder.Build(logicalTypeRegistry);

            _connectionString = Guard.NotNullOrWhiteSpace(connectionString, "connectionString");
        }

        protected override async Task<AggregateSnapshotLoadFrame> GetSnapshot(object id, int maxVersion)
        {
            string idString = ConvertIdentifierToString(id);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var dbAggregate = await GetAggregate(idString, connection);
                if (dbAggregate != null)
                {
                    Type aggregateType = _logicalTypeRegistry.LookupActualType(
                        LogicalType.Parse(dbAggregate.Type));

                    var command = new SqlCommand
                    {
                        Connection = connection,
                        CommandType = CommandType.Text,
                        CommandText = @"SELECT TOP 1 Data
                                          FROM AggregateSnapshots
                                         WHERE AggregateId = @pAggregateId 
                                           AND Version <= @pMaxVersion
                                      ORDER BY Version DESC"
                    };

                    command.Parameters.AddWithValue("pAggregateId", idString);
                    command.Parameters.AddWithValue("pMaxVersion", maxVersion);

                    using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow))
                    {
                        if (await reader.ReadAsync())
                        {
                            string data = reader.GetString(reader.GetOrdinal("Data"));

                            var snapshot = _dataSerializer.Deserialize<IAggregateSnapshot>(data);
                            return new AggregateSnapshotLoadFrame(aggregateType, id, snapshot);
                        }
                    }
                }

                return null;
            }
        }

        protected override async Task<AggregateEventsLoadFrame> GetEvents(object id, int minVersion, int maxVersion)
        {
            string idString = ConvertIdentifierToString(id);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var dbAggregate = await GetAggregate(idString, connection);

                if (dbAggregate != null)
                {
                    var dbEvents = await GetAggregateEvents(idString, minVersion, maxVersion, connection);

                    Type aggregateType = _logicalTypeRegistry.LookupActualType(
                        LogicalType.Parse(dbAggregate.Type));

                    var events = dbEvents
                        .Select(x => _dataSerializer.Deserialize<IAggregateEvent>(x))
                        .ToArray();

                    return events.Length > 0
                        ? new AggregateEventsLoadFrame(aggregateType, id, events)
                        : null;
                }

                return null;
            }
        }

        protected override async Task SaveSnapshot(AggregateSnapshotSaveFrame frame)
        {
            string idString = ConvertIdentifierToString(frame.Id);
            string type = _logicalTypeRegistry.LookupLogicalType(frame.Snapshot.GetType()).ToString();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    if (frame.IsNew)
                        await InsertAggregate(connection, transaction, idString, frame.Snapshot.Version, type);

                    var command = new SqlCommand
                    {
                        Connection = connection,
                        Transaction = transaction,
                        CommandType = CommandType.Text,
                        CommandText = @"INSERT INTO AggregateSnapshots (AggregateId, Version, Type, Data) VALUES (@pAggregateId, @pVersion, @pType, @pData)"
                    };

                    command.Parameters.AddWithValue("pAggregateId", idString);
                    command.Parameters.AddWithValue("pVersion", frame.Snapshot.Version);
                    command.Parameters.AddWithValue("pType", type);
                    command.Parameters.AddWithValue("pData", _dataSerializer.Serialize(frame.Snapshot));

                    try
                    {
                        int recordsAffected = await command.ExecuteNonQueryAsync();
                        if (recordsAffected != 1)
                            throw new AggregateConcurrencyException();
                    }
                    catch (SqlException sqlEx)
                    {
                        // catch primary key constraint violations - saving snapshot is idempotent
                        if (sqlEx.Number != 2627)
                            throw;
                    }

                    transaction.Commit();
                }
            }
        }

        protected override async Task SaveEvents(AggregateEventsSaveFrame frame)
        {
            string idString = ConvertIdentifierToString(frame.Id);
            string type = _logicalTypeRegistry.LookupLogicalType(frame.AggregateType).ToString();
            var events = frame.Events.ToArray();

            if (events.Length <= 0)
                throw new ArgumentException("There are no events to save.", "frame");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    await UpsertAggregate(connection, transaction, idString, events[0].Version - 1, events[events.Length - 1].Version, type);
                    await InsertEvents(events, idString, connection, transaction);

                    transaction.Commit();
                }
            }
        }

        private static async Task UpsertAggregate(SqlConnection connection, SqlTransaction transaction, string idString, int expectedVersion, int newVersion, string type)
        {
            bool isNewAggregate = expectedVersion == 0;
            if (isNewAggregate)
            {
                await InsertAggregate(connection, transaction, idString, newVersion, type);
            }
            else
            {
                var updateCommand = new SqlCommand
                {
                    Connection = connection,
                    Transaction = transaction,
                    CommandType = CommandType.Text,
                    CommandText =
                        "UPDATE Aggregates SET Version = @pVersion WHERE AggregateId = @pAggregateId AND Version = @pExpectedVersion"
                };

                updateCommand.Parameters.AddWithValue("pAggregateId", idString);
                updateCommand.Parameters.AddWithValue("pVersion", newVersion);
                updateCommand.Parameters.AddWithValue("pExpectedVersion", expectedVersion);

                int recordsAffected = await updateCommand.ExecuteNonQueryAsync();
                if (recordsAffected != 1)
                    throw new AggregateConcurrencyException();
            }
        }

        private static async Task InsertAggregate(SqlConnection connection, SqlTransaction transaction, string idString, int newVersion, string type)
        {
            var insertCommand = new SqlCommand
            {
                Connection = connection,
                Transaction = transaction,
                CommandType = CommandType.Text,
                CommandText =
                    "INSERT INTO Aggregates (AggregateId, Version, Type) VALUES (@pAggregateId, @pVersion, @pType)"
            };

            insertCommand.Parameters.AddWithValue("pAggregateId", idString);
            insertCommand.Parameters.AddWithValue("pVersion", newVersion);
            insertCommand.Parameters.AddWithValue("pType", type);

            int recordsAffected = await insertCommand.ExecuteNonQueryAsync();
            if (recordsAffected != 1)
                throw new AggregateConcurrencyException();
        }

        private async Task InsertEvents(IEnumerable<IAggregateEvent> events, string idString, SqlConnection connection, SqlTransaction transaction)
        {
            var dbEvents = events.Select(x => new DbAggregateEvent(idString,
                                                                   x.Version,
                                                                   _logicalTypeRegistry.LookupLogicalType(x.GetType()).ToString(),
                                                                   _dataSerializer.Serialize(x)))
                .ToArray();

            foreach (var dbEvent in dbEvents)
            {
                int recordsAffected = await InsertEvent(connection, transaction, dbEvent);
                if (recordsAffected != 1)
                    throw new AggregateConcurrencyException();
            }
        }

        private async Task<int> InsertEvent(SqlConnection connection, SqlTransaction transaction, DbAggregateEvent @event)
        {
            var command = new SqlCommand
            {
                Connection = connection,
                Transaction = transaction,
                CommandType = CommandType.Text,
                CommandText = "INSERT INTO AggregateEvents (AggregateId, Version, Type, Data) VALUES(@pAggregateId, @pVersion, @pType, @pData)"
            };

            command.Parameters.AddWithValue("pAggregateId", @event.AggregateId);
            command.Parameters.AddWithValue("pVersion", @event.Version);
            command.Parameters.AddWithValue("pType", @event.Type);
            command.Parameters.AddWithValue("pData", @event.Data);

            return await command.ExecuteNonQueryAsync();
        }

        private async Task<DbAggregate> GetAggregate(string id, SqlConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = @"SELECT Version, Type 
                                      FROM Aggregates
                                     WHERE AggregateId = @pAggregateId";

            command.Parameters.AddWithValue("pAggregateId", id);

            using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow))
            {
                if (await reader.ReadAsync())
                {
                    int version = reader.GetInt32(reader.GetOrdinal("Version"));
                    string type = reader.GetString(reader.GetOrdinal("Type"));

                    return new DbAggregate(id, version, type);
                }

                return null;
            }
        }

        private async Task<String[]> GetAggregateEvents(string id, int minVersion, int maxVersion, SqlConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = @"SELECT Data 
                                      FROM AggregateEvents 
                                     WHERE AggregateId = @pAggregateId 
                                       AND Version >= @pMinVersion
                                       AND Version <= @pMaxVersion
                                     ORDER BY Version";


            command.Parameters.AddWithValue("pAggregateId", id);
            command.Parameters.AddWithValue("pMinVersion", minVersion);
            command.Parameters.AddWithValue("pMaxVersion", maxVersion);

            var results = new List<String>();

            using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
            {
                while (await reader.ReadAsync())
                {
                    string data = reader.GetString(reader.GetOrdinal("Data"));

                    results.Add(data);
                }
            }

            return results.ToArray();
        }

        protected virtual string ConvertIdentifierToString(object id)
        {
            Guard.NotNull(id, "id");
            return _dataSerializer.Serialize(id);
        }
    }

    public class SqlServerAggregateRepository<TAggregate, TIdentifier> : AggregateRepository<TAggregate, TIdentifier>
        where TAggregate : class, IAggregate<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        public SqlServerAggregateRepository(ILogicalTypeRegistry logicalTypeRegistry, string connectionString)
            :base(new SqlServerAggregateRepository(logicalTypeRegistry, connectionString))
        {
        }
    }
}