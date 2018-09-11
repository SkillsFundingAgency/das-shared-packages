using System;
using NServiceBus.Persistence.Sql;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.NServiceBus.SqlServer.ClientOutbox
{
    public static class ClientOutboxTransactionExtensions
    {
        public static ISqlStorageSession SqlPersistenceSession(this IClientOutboxTransaction clientOutboxTransaction)
        {
            switch (clientOutboxTransaction)
            {
                case null:
                    throw new ArgumentNullException(nameof(clientOutboxTransaction));
                case ISqlStorageSession sqlStorageSession:
                    return sqlStorageSession;
                default:
                    throw new Exception("Cannot access the SQL storage session");
            }
        }
    }
}