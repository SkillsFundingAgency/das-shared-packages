using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus.Persistence;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.NServiceBus.SqlServer.ClientOutbox
{
    /// <summary>
    /// Based on the OutboxPersister implementation in NServiceBus
    /// https://github.com/Particular/NServiceBus.Persistence.Sql/blob/4.1.1/src/SqlPersistence/Outbox/OutboxPersister.cs
    /// </summary>
    public class ClientOutboxPersister : IClientOutboxStorage
    {
        public const string GetCommandText = "SELECT EndpointName, Operations FROM dbo.ClientOutboxData WHERE MessageId = @MessageId";
        public const string SetAsDispatchedCommandText = "UPDATE dbo.ClientOutboxData SET Dispatched = 1, DispatchedAt = @DispatchedAt, Operations = '[]' WHERE MessageId = @MessageId";
        public const string GetAwaitingDispatchCommandText = "SELECT MessageId, EndpointName FROM dbo.ClientOutboxData WHERE CreatedAt <= @CreatedAt AND Dispatched = 0 ORDER BY CreatedAt";
        public const string StoreCommandText = "INSERT INTO dbo.ClientOutboxData (MessageId, EndpointName, CreatedAt, Operations) VALUES (@MessageId, @EndpointName, @CreatedAt, @Operations)";

        private readonly DbConnection _connection;

        public ClientOutboxPersister(DbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IClientOutboxTransaction> BeginTransactionAsync()
        {
            await _connection.OpenAsync().ConfigureAwait(false);

            var transaction = _connection.BeginTransaction();
            var sqlClientOutboxTransaction = new SqlClientOutboxTransaction(_connection, transaction);

            return sqlClientOutboxTransaction;
        }

        public async Task<ClientOutboxMessage> GetAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession)
        {
            var sqlSession = synchronizedStorageSession.GetSqlSession();

            using (var command = sqlSession.Connection.CreateCommand())
            {
                command.CommandText = GetCommandText;
                command.CommandType = CommandType.Text;
                command.Transaction = sqlSession.Transaction;
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
            await _connection.OpenAsync().ConfigureAwait(false);

            try
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = GetAwaitingDispatchCommandText;
                    command.CommandType = CommandType.Text;
                    command.AddParameter("CreatedAt", DateTime.UtcNow.AddMinutes(-10));

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
            finally
            {
                _connection.Close();
            }
        }

        public Task SetAsDispatchedAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession)
        {
            var sqlSession = synchronizedStorageSession.GetSqlSession();

            using (var command = sqlSession.Connection.CreateCommand())
            {
                command.CommandText = SetAsDispatchedCommandText;
                command.CommandType = CommandType.Text;
                command.Transaction = sqlSession.Transaction;
                command.AddParameter("MessageId", messageId);
                command.AddParameter("DispatchedAt", DateTime.UtcNow);

                return command.ExecuteNonQueryAsync();
            }
        }

        public Task StoreAsync(ClientOutboxMessage clientOutboxMessage, IClientOutboxTransaction clientOutboxTransaction)
        {
            var sqlClientOutboxTransaction = (SqlClientOutboxTransaction)clientOutboxTransaction;
            var transaction = sqlClientOutboxTransaction.Transaction;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = StoreCommandText;
                command.CommandType = CommandType.Text;
                command.Transaction = transaction;
                command.AddParameter("MessageId", clientOutboxMessage.MessageId);
                command.AddParameter("EndpointName", clientOutboxMessage.EndpointName);
                command.AddParameter("CreatedAt", DateTime.UtcNow);
                command.AddParameter("Operations", JsonConvert.SerializeObject(clientOutboxMessage.Operations, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));

                return command.ExecuteNonQueryAsync();
            }
        }
    }
}