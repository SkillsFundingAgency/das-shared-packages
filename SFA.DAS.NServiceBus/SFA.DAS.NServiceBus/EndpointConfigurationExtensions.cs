using System;
using NServiceBus;

namespace SFA.DAS.NServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupErrorQueue(this EndpointConfiguration config)
        {
            config.SendFailedMessagesTo("errors");

            return config;
        }

        public static EndpointConfiguration SetupHeartbeat(this EndpointConfiguration config)
        {
            config.SendHeartbeatTo("heartbeats");

            return config;
        }

        public static EndpointConfiguration SetupInstallers(this EndpointConfiguration config)
        {
            config.EnableInstallers();

            return config;
        }

        public static EndpointConfiguration SetupMetrics(this EndpointConfiguration config)
        {
            var metrics = config.EnableMetrics();

            metrics.SendMetricDataToServiceControl("particular.monitoring", TimeSpan.FromSeconds(10));

            return config;
        }

        public static EndpointConfiguration SetupOutbox(this EndpointConfiguration config)
        {
            var outbox = config.EnableOutbox();

            outbox.KeepDeduplicationDataFor(TimeSpan.FromDays(28));
            outbox.RunDeduplicationDataCleanupEvery(TimeSpan.FromDays(1));

            return config;
        }

        public static EndpointConfiguration SetupPurgeOnStartup(this EndpointConfiguration config)
        {
            config.PurgeOnStartup(true);

            return config;
        }

        public static EndpointConfiguration SetupSendOnly(this EndpointConfiguration config)
        {
            config.SendOnly();

            return config;
        }

        public static EndpointConfiguration SetupUnitOfWork(this EndpointConfiguration config)
        {
            config.RegisterComponents(c =>
            {
                if (!c.HasComponent<IDb>())
                {
                    c.ConfigureComponent<Db>(DependencyLifecycle.InstancePerUnitOfWork);
                }
            });

            config.Pipeline.Register(new UnitOfWorkBehavior(), "Sets up a unit of work for each message");

            return config;
        }
    }
}