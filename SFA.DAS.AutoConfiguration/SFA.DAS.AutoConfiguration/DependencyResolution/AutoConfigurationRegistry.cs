using StructureMap;

namespace SFA.DAS.AutoConfiguration.DependencyResolution
{
    public class AutoConfigurationRegistry : Registry
    {
        public AutoConfigurationRegistry()
        {
            For<IAutoConfigurationService>().Use<TableStorageConfigurationService>();
            For<IEnvironmentService>().Use<EnvironmentService>();
            For<IAzureTableStorageConnectionAdapter>().Use<AzureTableStorageConnectionAdapter>();
        }
    }
}
