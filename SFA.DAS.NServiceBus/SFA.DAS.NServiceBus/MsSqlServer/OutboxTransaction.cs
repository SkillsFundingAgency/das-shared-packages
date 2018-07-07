using System.Data.Common;

namespace SFA.DAS.NServiceBus.MsSqlServer
{
    public class OutboxTransaction : IOutboxTransaction
    {
        private readonly DbTransaction _transaction;

        public OutboxTransaction(DbTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}