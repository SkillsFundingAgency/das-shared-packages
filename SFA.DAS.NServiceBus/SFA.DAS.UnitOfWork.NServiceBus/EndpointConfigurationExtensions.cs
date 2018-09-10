using NServiceBus;

namespace SFA.DAS.UnitOfWork.NServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseUnitOfWork(this EndpointConfiguration config)
        {
            config.EnableUniformSession();

            config.RegisterComponents(c =>
            {
                c.ConfigureComponent<EventPublisher>(DependencyLifecycle.InstancePerUnitOfWork);
            });
            
            config.Pipeline.Register(new UnitOfWorkBehavior(), "Sets up a unit of work for each message");
            config.Pipeline.Register(new UnitOfWorkContextBehavior(), "Sets up a unit of work context for each message");

            return config;
        }
    }
}