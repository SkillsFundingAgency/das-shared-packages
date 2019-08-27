using System;
using System.Data.Common;
using System.Threading.Tasks;
using NServiceBus.Persistence.Sql;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;

namespace SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox.Data
{
    public class SqlClientOutboxTransaction : IClientOutboxTransaction, ISqlStorageSession
    {
        public DbConnection Connection { get; }
        public DbTransaction Transaction { get; }

        public SqlClientOutboxTransaction(DbConnection connection, DbTransaction transaction)
        {
            Connection = connection;
            Transaction = transaction;
        }

        public Task CommitAsync()
        {
            Transaction.Commit();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Transaction.Dispose();
            Connection.Dispose();
        }

        public void OnSaveChanges(Func<ISqlStorageSession, Task> callback)
        {
            throw new NotImplementedException();
        }
    }
}