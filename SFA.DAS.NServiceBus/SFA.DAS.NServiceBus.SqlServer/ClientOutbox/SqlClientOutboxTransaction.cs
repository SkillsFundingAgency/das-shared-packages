using System;
using System.Data.Common;
using System.Threading.Tasks;
using NServiceBus.Persistence.Sql;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.NServiceBus.SqlServer.ClientOutbox
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