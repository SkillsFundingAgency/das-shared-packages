using NServiceBus;
using StructureMap;

namespace SFA.DAS.NServiceBus.StructureMap
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupStructureMapBuilder(this EndpointConfiguration config, IContainer container)
        {
            container.Configure(c => c.AddRegistry<NServiceBusRegistry>());
            config.UseContainer<StructureMapBuilder>(c => c.ExistingContainer(container));

            return config;
        }
    }
}