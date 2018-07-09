using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.NServiceBus.MsSqlServer
{
    /// <summary>
    /// Based on the OutboxPersister implementation in NServiceBus
    /// https://github.com/Particular/NServiceBus.Persistence.Sql/blob/4.1.1/src/SqlPersistence/Outbox/OutboxPersister.cs
    /// </summary>
    public class Outbox : IOutbox
    {
        public const string GetCommandText = "SELECT EndpointName, Operations FROM dbo.ClientOutboxData WHERE MessageId = @MessageId";
        public const string SetAsDispatchedCommandText = "UPDATE dbo.ClientOutboxData SET Dispatched = 1, DispatchedAt = @DispatchedAt, Operations = '[]' WHERE MessageId = @MessageId";
        public const string GetAwaitingDispatchCommandText = "SELECT MessageId, EndpointName FROM dbo.ClientOutboxData WHERE Created <= @Created AND Dispatched = 0 ORDER BY Created";
        public const string StoreCommandText = "INSERT INTO dbo.ClientOutboxData (MessageId, EndpointName, Created, Operations) VALUES (@MessageId, @EndpointName, @Created, @Operations)";

        private readonly DbConnection _connection;
        private readonly IUnitOfWorkContext _unitOfWorkContext;

        public Outbox(DbConnection connection, IUnitOfWorkContext unitOfWorkContext)
        {
            _unitOfWorkContext = unitOfWorkContext;
            _connection = connection;
        }

        public async Task<IOutboxTransaction> BeginTransactionAsync()
        {
            await _connection.TryOpenAsync().ConfigureAwait(false);

            var transaction = _connection.BeginTransaction();
            var outboxTransaction = new OutboxTransaction(transaction);

            _unitOfWorkContext.Set(_connection);
            _unitOfWorkContext.Set(transaction);

            return outboxTransaction;
        }

        public async Task<OutboxMessage> GetAsync(Guid messageId)
        {
            var connection = _unitOfWorkContext.Get<DbConnection>();
            var transaction = _unitOfWorkContext.Get<DbTransaction>();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = GetCommandText;
                command.CommandType = CommandType.Text;
                command.Transaction = transaction;
                command.AddParameter("MessageId", messageId);

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var endpointName = reader.GetString(0);
                        var operations = JsonConvert.DeserializeObject<List<Event>>(reader.GetString(1), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                        var outboxMessage = new OutboxMessage(messageId, endpointName, operations);

                        return outboxMessage;
                    }

                    throw new KeyNotFoundException($"Client outbox data not found where MessageId = '{messageId}'");
                }
            }
        }

        public async Task<IEnumerable<IOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync()
        {
            await _connection.TryOpenAsync().ConfigureAwait(false);

            try
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = GetAwaitingDispatchCommandText;
                    command.CommandType = CommandType.Text;
                    command.AddParameter("Created", DateTime.UtcNow.AddMinutes(-10));

                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        var outboxMessages = new List<IOutboxMessageAwaitingDispatch>();

                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var messageId = reader.GetGuid(0);
                            var endpointName = reader.GetString(1);
                            var outboxMessage = new OutboxMessage(messageId, endpointName);

                            outboxMessages.Add(outboxMessage);
                        }

                        return outboxMessages;
                    }
                }
            }
            finally
            {
                _connection.Close();
            }
        }

        public Task SetAsDispatchedAsync(Guid messageId)
        {
            var connection = _unitOfWorkContext.Get<DbConnection>();
            var transaction = _unitOfWorkContext.Get<DbTransaction>();
            
            using (var command = connection.CreateCommand())
            {
                command.CommandText = SetAsDispatchedCommandText;
                command.CommandType = CommandType.Text;
                command.Transaction = transaction;
                command.AddParameter("MessageId", messageId);
                command.AddParameter("DispatchedAt", DateTime.UtcNow);

                return command.ExecuteNonQueryAsync();
            }
        }

        public Task StoreAsync(OutboxMessage outboxMessage, IOutboxTransaction outboxTransaction)
        {
            var msSqlOutboxTransaction = (OutboxTransaction)outboxTransaction;
            var transaction = msSqlOutboxTransaction.Transaction;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = StoreCommandText;
                command.CommandType = CommandType.Text;
                command.Transaction = transaction;
                command.AddParameter("MessageId", outboxMessage.MessageId);
                command.AddParameter("EndpointName", outboxMessage.EndpointName);
                command.AddParameter("Created", DateTime.UtcNow);
                command.AddParameter("Operations", JsonConvert.SerializeObject(outboxMessage.Operations, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));

                return command.ExecuteNonQueryAsync();
            }
        }
    }
}