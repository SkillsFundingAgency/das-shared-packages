using StructureMap;

namespace SFA.DAS.AutoConfiguration.DependencyResolution
{
    public class AutoConfigurationRegistry : Registry
    {
        public AutoConfigurationRegistry()
        {
            For<ITableStorageConfigurationService>().Use<TableStorageConfigurationService>();
            For<IEnvironmentService>().Use<EnvironmentService>();
            For<IAzureTableStorageConnectionAdapter>().Use<AzureTableStorageConnectionAdapter>();
        }
    }
}
