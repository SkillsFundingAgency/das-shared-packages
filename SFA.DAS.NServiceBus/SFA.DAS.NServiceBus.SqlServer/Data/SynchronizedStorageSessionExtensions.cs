using System;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;

namespace SFA.DAS.NServiceBus.SqlServer.Data
{
    public static class SynchronizedStorageSessionExtensions
    {
        public static ISqlStorageSession GetSqlStorageSession(this SynchronizedStorageSession synchronizedStorageSession)
        {
            if (synchronizedStorageSession is ISqlStorageSession sqlStorageSession)
            {
                return sqlStorageSession;
            }

            throw new Exception("Cannot access the SQL storage session");
        }
    }
}