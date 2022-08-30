using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox;
using System;
using System.Data.Common;

namespace SFA.DAS.NServiceBus.SqlServer.Configuration
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
            
            config.EnableFeature<SqlServerClientOutboxFeature>();

            return config;
        }
        
        public static EndpointConfiguration UseSqlServerPersistence(
            this EndpointConfiguration config,         
            Func<DbConnection> connectionBuilder)
        {            
            var persistence = config.UsePersistence<SqlPersistence>();
            persistence.ConnectionBuilder(connectionBuilder);
            persistence.GetSettings().Set("SqlPersistence.ConnectionBuilder", connectionBuilder);
            persistence.DisableInstaller();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.TablePrefix("");
            
            return config;
        }

    }
}