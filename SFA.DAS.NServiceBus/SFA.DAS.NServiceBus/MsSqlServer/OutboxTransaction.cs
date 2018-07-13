using System.Data.Common;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.MsSqlServer
{
    public class OutboxTransaction : IOutboxTransaction
    {
        public DbTransaction Transaction { get; }

        public OutboxTransaction(DbTransaction transaction)
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