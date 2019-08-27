using NServiceBus;
using SFA.DAS.UnitOfWork.NServiceBus.Behaviors;

namespace SFA.DAS.UnitOfWork.NServiceBus.Configuration
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseUnitOfWork(this EndpointConfiguration config)
        {
            config.EnableUniformSession();
            config.Pipeline.Register(new UnitOfWorkBehavior(), "Sets up a unit of work for each message");
            config.Pipeline.Register(new UnitOfWorkContextBehavior(), "Sets up a unit of work context for each message");

            return config;
        }
    }
}