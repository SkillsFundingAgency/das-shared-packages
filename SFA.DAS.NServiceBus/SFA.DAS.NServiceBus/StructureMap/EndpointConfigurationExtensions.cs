using NServiceBus;
using SFA.DAS.NServiceBus.ClientOutbox;
using StructureMap;

namespace SFA.DAS.NServiceBus.StructureMap
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseStructureMapBuilder(this EndpointConfiguration config, IContainer container)
        {
            container.Configure(c => c.For<IProcessClientOutboxMessagesJob>().Use<ProcessClientOutboxMessagesJob>().Transient());
            config.UseContainer<StructureMapBuilder>(c => c.ExistingContainer(container));

            return config;
        }
    }
}