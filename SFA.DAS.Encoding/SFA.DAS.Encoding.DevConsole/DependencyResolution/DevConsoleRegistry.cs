using SFA.DAS.AutoConfiguration;
using SFA.DAS.Encoding.DevConsole.Interfaces;
using StructureMap;

namespace SFA.DAS.Encoding.DevConsole.DependencyResolution
{
    public class DevConsoleRegistry : Registry
    {
        public DevConsoleRegistry()
        {
            var tableStorageConfigurationService = new TableStorageConfigurationService(
                new EnvironmentService(),
                new AzureTableStorageConnectionAdapter());
            var encodingConfig = tableStorageConfigurationService.Get<EncodingConfig>("SFA.DAS.Encoding");

            For<EncodingConfig>().Use(encodingConfig).Singleton();
            For<IEncodingService>().Use<EncodingService>().Singleton();
            For<IEncodingServiceFacade>().Use<EncodingServiceFacade>();
        }
    }
}