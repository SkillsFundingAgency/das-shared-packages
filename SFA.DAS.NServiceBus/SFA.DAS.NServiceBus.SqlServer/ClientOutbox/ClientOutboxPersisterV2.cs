﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus.Persistence;
using NServiceBus.Settings;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.NServiceBus.SqlServer.ClientOutbox
{
    /// <summary>
    /// Based on the OutboxPersister implementation in NServiceBus
    /// https://github.com/Particular/NServiceBus.Persistence.Sql/blob/4.1.1/src/SqlPersistence/Outbox/OutboxPersister.cs
    /// </summary>
    public class ClientOutboxPersisterV2 : IClientOutboxStorageV2
    {
        public const string GetCommandText = "SELECT EndpointName, Operations FROM dbo.ClientOutboxData WHERE MessageId = @MessageId";
        public const string SetAsDispatchedCommandText = "UPDATE dbo.ClientOutboxData SET Dispatched = 1, DispatchedAt = @DispatchedAt, Operations = '[]' WHERE MessageId = @MessageId";
        public const string GetAwaitingDispatchCommandText = "SELECT MessageId, EndpointName FROM dbo.ClientOutboxData WHERE CreatedAt <= @CreatedAt AND Dispatched = 0 AND PersistenceVersion = '2.0.0' ORDER BY CreatedAt";
        public const string StoreCommandText = "INSERT INTO dbo.ClientOutboxData (MessageId, EndpointName, CreatedAt, PersistenceVersion, Operations) VALUES (@MessageId, @EndpointName, @CreatedAt, '2.0.0', @Operations)";

        private readonly Func<DbConnection> _connectionBuilder;

        public ClientOutboxPersisterV2(ReadOnlySettings settings)
        {
            _connectionBuilder = settings.Get<Func<DbConnection>>("SqlPersistence.ConnectionBuilder");
        }

        public async Task<IClientOutboxTransaction> BeginTransactionAsync()
        {
            var connection = await _connectionBuilder.OpenConnectionAsync().ConfigureAwait(false);

            var transaction = connection.BeginTransaction();
            var sqlClientOutboxTransaction = new SqlClientOutboxTransaction(connection, transaction);

            return sqlClientOutboxTransaction;
        }

        public async Task<ClientOutboxMessageV2> GetAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession)
        {
            var sqlStorageSession = synchronizedStorageSession.GetSqlStorageSession();

            using (var command = sqlStorageSession.Connection.CreateCommand())
            {
                command.CommandText = GetCommandText;
                command.CommandType = CommandType.Text;
                command.Transaction = sqlStorageSession.Transaction;
                command.AddParameter("MessageId", messageId);

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var endpointName = reader.GetString(0);
                        var transportOperations = JsonConvert.DeserializeObject<List<TransportOperation>>(reader.GetString(1), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                        var clientOutboxMessage = new ClientOutboxMessageV2(messageId, endpointName, transportOperations);

                        return clientOutboxMessage;
                    }

                    throw new KeyNotFoundException($"Client outbox data not found where MessageId = '{messageId}'");
                }
            }
        }

        public async Task<IEnumerable<IClientOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync()
        {
            var connection = await _connectionBuilder.OpenConnectionAsync().ConfigureAwait(false);

            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetAwaitingDispatchCommandText;
                    command.CommandType = CommandType.Text;
                    command.AddParameter("CreatedAt", DateTime.UtcNow.AddSeconds(-10));

                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        var clientOutboxMessages = new List<IClientOutboxMessageAwaitingDispatch>();

                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var messageId = reader.GetGuid(0);
                            var endpointName = reader.GetString(1);
                            var clientOutboxMessage = new ClientOutboxMessageV2(messageId, endpointName);

                            clientOutboxMessages.Add(clientOutboxMessage);
                        }

                        return clientOutboxMessages;
                    }
                }
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task SetAsDispatchedAsync(Guid messageId)
        {
            var connection = await _connectionBuilder.OpenConnectionAsync().ConfigureAwait(false);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = SetAsDispatchedCommandText;
                command.CommandType = CommandType.Text;
                command.AddParameter("MessageId", messageId);
                command.AddParameter("DispatchedAt", DateTime.UtcNow);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public Task SetAsDispatchedAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession)
        {
            var sqlStorageSession = synchronizedStorageSession.GetSqlStorageSession();

            using (var command = sqlStorageSession.Connection.CreateCommand())
            {
                command.CommandText = SetAsDispatchedCommandText;
                command.CommandType = CommandType.Text;
                command.Transaction = sqlStorageSession.Transaction;
                command.AddParameter("MessageId", messageId);
                command.AddParameter("DispatchedAt", DateTime.UtcNow);

                return command.ExecuteNonQueryAsync();
            }
        }

        public Task StoreAsync(ClientOutboxMessageV2 clientOutboxMessage, IClientOutboxTransaction clientOutboxTransaction)
        {
            var sqlClientOutboxTransaction = (SqlClientOutboxTransaction)clientOutboxTransaction;

            using (var command = sqlClientOutboxTransaction.Connection.CreateCommand())
            {
                command.CommandText = StoreCommandText;
                command.CommandType = CommandType.Text;
                command.Transaction = sqlClientOutboxTransaction.Transaction;
                command.AddParameter("MessageId", clientOutboxMessage.MessageId);
                command.AddParameter("EndpointName", clientOutboxMessage.EndpointName);
                command.AddParameter("CreatedAt", DateTime.UtcNow);
                command.AddParameter("Operations", JsonConvert.SerializeObject(clientOutboxMessage.TransportOperations, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));

                return command.ExecuteNonQueryAsync();
            }
        }
    }
}