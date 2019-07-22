using StructureMap;

namespace SFA.DAS.AutoConfiguration.DependencyResolution
{
    public class AutoConfigurationRegistry : Registry
    {
        public AutoConfigurationRegistry()
        {
            For<IAutoConfigurationService>().Use<TableStorageConfigurationService>().Transient();
            For<IEnvironmentService>().Use<EnvironmentService>().Transient();
            For<IAzureTableStorageConnectionAdapter>().Use<AzureTableStorageConnectionAdapter>().Transient();
        }
    }
}
