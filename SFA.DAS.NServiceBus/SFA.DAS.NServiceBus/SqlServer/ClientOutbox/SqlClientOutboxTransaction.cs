using System.Data.Common;
using System.Threading.Tasks;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.NServiceBus.SqlServer.ClientOutbox
{
    public class SqlClientOutboxTransaction : IClientOutboxTransaction
    {
        public DbTransaction Transaction { get; }

        public SqlClientOutboxTransaction(DbTransaction transaction)
        {
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
        }
    }
}