using System;
using System.Data.Common;
using NServiceBus;
using NServiceBus.Persistence.Sql;

namespace SFA.DAS.NServiceBus.MsSqlServer
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupMsSqlServerPersistence(this EndpointConfiguration config, Func<DbConnection> connectionBuilder)
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
                c.ConfigureComponent<Outbox>(DependencyLifecycle.InstancePerUnitOfWork);
            });

            return config;
        }
    }
}