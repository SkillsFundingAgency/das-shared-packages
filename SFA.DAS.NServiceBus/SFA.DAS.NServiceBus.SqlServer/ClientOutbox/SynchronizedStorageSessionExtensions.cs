using System;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;

namespace SFA.DAS.NServiceBus.SqlServer.ClientOutbox
{
    public static class SynchronizedStorageSessionExtensions
    {
        public static ISqlStorageSession GetSqlSession(this SynchronizedStorageSession synchronizedStorageSession)
        {
            if (synchronizedStorageSession is ISqlStorageSession sqlSession)
            {
                return sqlSession;
            }

            throw new Exception("Cannot access the SQL session");
        }
    }
}