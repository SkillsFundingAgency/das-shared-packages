using NServiceBus;
using StructureMap;

namespace SFA.DAS.NServiceBus.StructureMap
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseStructureMapBuilder(this EndpointConfiguration config, IContainer container)
        {
            config.UseContainer<StructureMapBuilder>(c => c.ExistingContainer(container));

            return config;
        }
    }
}