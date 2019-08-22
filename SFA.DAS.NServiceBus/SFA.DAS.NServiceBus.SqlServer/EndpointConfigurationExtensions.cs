using System;
using System.Data.Common;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using SFA.DAS.NServiceBus.SqlServer.ClientOutbox;

namespace SFA.DAS.NServiceBus.SqlServer
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseOutbox(this EndpointConfiguration config, bool enableCleanup = false, TimeSpan? cleanupFrequency = null, TimeSpan? cleanupMaxAge = null)
        {
            var outbox = config.EnableOutbox();

            if (!enableCleanup)
            {
                outbox.DisableCleanup();
            }

            if (cleanupFrequency != null)
            {
                outbox.RunDeduplicationDataCleanupEvery(cleanupFrequency.Value);
            }

            if (cleanupMaxAge != null)
            {
                outbox.KeepDeduplicationDataFor(cleanupMaxAge.Value);
            }

            config.EnableFeature<SqlClientOutboxFeature>();
            
            return config;
        }
        
        public static EndpointConfiguration UseSqlServerPersistence(this EndpointConfiguration config, Func<DbConnection> connectionBuilder)
        {
            var persistence = config.UsePersistence<SqlPersistence>();

            persistence.ConnectionBuilder(connectionBuilder);
            persistence.DisableInstaller();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.TablePrefix("");
            
            return config;
        }
    }
}