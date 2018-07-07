using System;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;

namespace SFA.DAS.NServiceBus.MsSqlServer
{
    public static class SynchronizedStorageSessionExtensions
    {
        public static ISqlStorageSession GetSqlStorageSession(this SynchronizedStorageSession storageSession)
        {
            if (storageSession is ISqlStorageSession sqlStorageSession)
            {
                return sqlStorageSession;
            }

            throw new Exception("Cannot access the SQL synchronized storage session. Either this endpoint has not been configured to use the SQL persistence or a different persistence type is used for sagas.");
        }
    }
}