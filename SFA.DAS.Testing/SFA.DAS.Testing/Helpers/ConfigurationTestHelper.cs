using SFA.DAS.AutoConfiguration;

namespace SFA.DAS.Testing.Helpers
{
    public class ConfigurationTestHelper
    {
        public static T GetConfiguration<T>(string rowKey)
        {
            var configurationService = CreateConfigurationService();
            return configurationService.Get<T>(rowKey);
        }

        private static IAutoConfigurationService CreateConfigurationService()
        {
            return new TableStorageConfigurationService(new EnvironmentService(), new AzureTableStorageConnectionAdapter());
        }
    }
}
