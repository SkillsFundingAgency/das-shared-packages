using System;
using System.Data.Common;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Persistence.Sql;
using SFA.DAS.NServiceBus.SqlServer.ClientOutbox;

namespace SFA.DAS.NServiceBus.SqlServer
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseSqlServerPersistence(this EndpointConfiguration config, Func<DbConnection> connectionBuilder)
        {
            config.GetSettings().Set("Persistence.Sql.Outbox.DisableCleanup", true);

            config.RegisterComponents(c =>
            {
                c.ConfigureComponent<ClientOutboxPersister>(DependencyLifecycle.InstancePerUnitOfWork);
                c.ConfigureComponent<ClientOutboxPersisterV2>(DependencyLifecycle.InstancePerUnitOfWork);
            });

            var persistence = config.UsePersistence<SqlPersistence>();

            persistence.ConnectionBuilder(connectionBuilder);
            persistence.DisableInstaller();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.TablePrefix("");

            return config;
        }
    }
}