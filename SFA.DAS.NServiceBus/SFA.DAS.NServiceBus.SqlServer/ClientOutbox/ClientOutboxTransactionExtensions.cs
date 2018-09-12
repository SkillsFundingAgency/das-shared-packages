using System;
using NServiceBus.Persistence.Sql;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.NServiceBus.SqlServer.ClientOutbox
{
    public static class ClientOutboxTransactionExtensions
    {
        public static ISqlStorageSession GetSqlSession(this IClientOutboxTransaction clientOutboxTransaction)
        {
            if (clientOutboxTransaction is ISqlStorageSession sqlSession)
            {
                return sqlSession;
            }

            throw new Exception("Cannot access the SQL session");
        }
    }
}