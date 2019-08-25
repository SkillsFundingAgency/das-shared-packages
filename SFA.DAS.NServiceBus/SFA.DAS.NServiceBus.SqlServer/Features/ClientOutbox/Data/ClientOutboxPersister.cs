using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus.Persistence;
using NServiceBus.Settings;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Models;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.NServiceBus.SqlServer.Data;

namespace SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox.Data
{
    /// <summary>
    /// Based on the OutboxPersister implementation in NServiceBus
    /// https://github.com/Particular/NServiceBus.Persistence.Sql/blob/4.1.1/src/SqlPersistence/Outbox/OutboxPersister.cs
    /// </summary>
    public class ClientOutboxPersister : IClientOutboxStorage
    {
        public const string GetCommandText = "SELECT EndpointName, Operations FROM dbo.ClientOutboxData WHERE MessageId = @MessageId";
        public const string SetAsDispatchedCommandText = "UPDATE dbo.ClientOutboxData SET Dispatched = 1, DispatchedAt = @DispatchedAt, Operations = '[]' WHERE MessageId = @MessageId";
        public const string GetAwaitingDispatchCommandText = "SELECT MessageId, EndpointName FROM dbo.ClientOutboxData WHERE Dispatched = 0 AND CreatedAt <= @CreatedAt AND PersistenceVersion = '1.0.0' ORDER BY CreatedAt";
        public const string StoreCommandText = "INSERT INTO dbo.ClientOutboxData (MessageId, EndpointName, CreatedAt, PersistenceVersion, Operations) VALUES (@MessageId, @EndpointName, @CreatedAt, '1.0.0', @Operations)";
        public const string RemoveEntriesOlderThanCommandText = "DELETE TOP(@BatchSize) FROM dbo.ClientOutboxData WHERE Dispatched = 1 AND DispatchedAt < @DispatchedBefore AND PersistenceVersion = '1.0.0' ORDER BY DispatchedAt";
        public const int CleanupBatchSize = 10000;
        
        private readonly IDateTimeService _dateTimeService;
        private readonly Func<DbConnection> _connectionBuilder;

        public ClientOutboxPersister(IDateTimeService dateTimeService, ReadOnlySettings settings)
        {
            _dateTimeService = dateTimeService;
            _connectionBuilder = settings.Get<Func<DbConnection>>("SqlPersistence.ConnectionBuilder");
        }

        public async Task<IClientOutboxTransaction> BeginTransactionAsync()
        {
            var connection = await _connectionBuilder.OpenConnectionAsync().ConfigureAwait(false);
            var transaction = connection.BeginTransaction();
            var sqlClientOutboxTransaction = new SqlClientOutboxTransaction(connection, transaction);

            return sqlClientOutboxTransaction;
        }

        public async Task<ClientOutboxMessage> GetAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession)
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
                        var operations = JsonConvert.DeserializeObject<List<object>>(reader.GetString(1), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                        var clientOutboxMessage = new ClientOutboxMessage(messageId, endpointName, operations);

                        return clientOutboxMessage;
                    }

                    throw new KeyNotFoundException($"Client outbox data not found where MessageId = '{messageId}'");
                }
            }
        }

        public async Task<IEnumerable<IClientOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync()
        {
            using (var connection = await _connectionBuilder.OpenConnectionAsync().ConfigureAwait(false))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = GetAwaitingDispatchCommandText;
                command.CommandType = CommandType.Text;
                command.AddParameter("CreatedAt", _dateTimeService.UtcNow.AddMinutes(-10));

                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    var clientOutboxMessages = new List<IClientOutboxMessageAwaitingDispatch>();

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var messageId = reader.GetGuid(0);
                        var endpointName = reader.GetString(1);
                        var clientOutboxMessage = new ClientOutboxMessage(messageId, endpointName);

                        clientOutboxMessages.Add(clientOutboxMessage);
                    }

                    return clientOutboxMessages;
                }
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
                command.AddParameter("DispatchedAt", _dateTimeService.UtcNow);

                return command.ExecuteNonQueryAsync();
            }
        }

        public Task StoreAsync(ClientOutboxMessage clientOutboxMessage, IClientOutboxTransaction clientOutboxTransaction)
        {
            var sqlClientOutboxTransaction = (SqlClientOutboxTransaction)clientOutboxTransaction;
            
            using (var command = sqlClientOutboxTransaction.Connection.CreateCommand())
            {
                command.CommandText = StoreCommandText;
                command.CommandType = CommandType.Text;
                command.Transaction = sqlClientOutboxTransaction.Transaction;
                command.AddParameter("MessageId", clientOutboxMessage.MessageId);
                command.AddParameter("EndpointName", clientOutboxMessage.EndpointName);
                command.AddParameter("CreatedAt", _dateTimeService.UtcNow);
                command.AddParameter("Operations", JsonConvert.SerializeObject(clientOutboxMessage.Operations, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));

                return command.ExecuteNonQueryAsync();
            }
        }

        public async Task RemoveEntriesOlderThanAsync(DateTime oldest, CancellationToken cancellationToken)
        {
            using (var connection = await _connectionBuilder.OpenConnectionAsync().ConfigureAwait(false))
            {
                var isComplete = false;
                
                while (!cancellationToken.IsCancellationRequested && !isComplete)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = RemoveEntriesOlderThanCommandText;
                        command.AddParameter("DispatchedBefore", oldest);
                        command.AddParameter("BatchSize", CleanupBatchSize);
                        
                        var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        
                        isComplete = rowsAffected < CleanupBatchSize;
                    }
                }
            }
        }
    }
}