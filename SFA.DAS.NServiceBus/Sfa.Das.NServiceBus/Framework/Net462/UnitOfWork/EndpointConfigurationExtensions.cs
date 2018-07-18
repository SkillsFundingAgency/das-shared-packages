using System;
using NServiceBus;

namespace SFA.DAS.NServiceBus.UnitOfWork
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupUnitOfWork(this EndpointConfiguration config)
        {
            config.RegisterComponents(c =>
            {
                if (!c.HasComponent<IDb>())
                {
                    c.ConfigureComponent<Db>(DependencyLifecycle.InstancePerUnitOfWork);
                }

                c.ConfigureComponent<EventPublisher>(DependencyLifecycle.InstancePerUnitOfWork);
                c.ConfigureComponent<UnitOfWorkContext>(DependencyLifecycle.InstancePerUnitOfWork);
                c.ConfigureComponent<UnitOfWorkManager>(DependencyLifecycle.InstancePerUnitOfWork);
            });

            config.Pipeline.Register(new UnitOfWorkBehavior(), "Sets up a unit of work for each message");

            return config;
        }
    }
}