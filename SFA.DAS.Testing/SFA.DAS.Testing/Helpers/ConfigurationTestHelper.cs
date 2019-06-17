using SFA.DAS.AutoConfiguration;

namespace SFA.DAS.Testing.Helpers
{
    public class ConfigurationTestHelper
    {
        private static IAutoConfigurationService _configurationService;

        public static T GetConfiguration<T>(string rowKey)
        {
            if(_configurationService == null)
                _configurationService = CreateConfigurationService();

            return _configurationService.Get<T>(rowKey);
        }

        private static IAutoConfigurationService CreateConfigurationService()
        {
            return new TableStorageConfigurationService(new EnvironmentService(), new AzureTableStorageConnectionAdapter());
        }
    }
}
