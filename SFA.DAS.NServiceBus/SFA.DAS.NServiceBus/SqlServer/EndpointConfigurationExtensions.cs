using System;
using System.Data.Common;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using SFA.DAS.NServiceBus.SqlServer.ClientOutbox;
using SFA.DAS.NServiceBus.SqlServer.UnitOfWork;

namespace SFA.DAS.NServiceBus.SqlServer
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupSqlServerPersistence(this EndpointConfiguration config, Func<DbConnection> connectionBuilder)
        {
            var persistence = config.UsePersistence<SqlPersistence>();

            persistence.ConnectionBuilder(connectionBuilder);
            persistence.DisableInstaller();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.TablePrefix("");

            config.Pipeline.Register(new IncomingPhysicalMessageBehavior(), "Sets up a unit of work context for each message");
            config.Pipeline.Register(new InvokeHandlerBehavior(), "Sets up a unit of work context for each message");

            config.RegisterComponents(c =>
            {
                c.ConfigureComponent<ClientOutboxPersister>(DependencyLifecycle.InstancePerUnitOfWork);
            });

            return config;
        }
    }
}