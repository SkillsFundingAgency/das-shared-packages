using System;
using SFA.DAS.AutoConfiguration;

namespace SFA.DAS.Testing.Helpers
{
    public class ConfigurationTestHelper
    {
        private static readonly Lazy<IAutoConfigurationService> LazyAutoConfigurationService = new Lazy<IAutoConfigurationService>(CreateConfigurationService);

        public static T GetConfiguration<T>(string rowKey)
        {
            return LazyAutoConfigurationService.Value.Get<T>(rowKey);
        }

        private static IAutoConfigurationService CreateConfigurationService()
        {
            return new TableStorageConfigurationService(new EnvironmentService(), new AzureTableStorageConnectionAdapter());
        }
    }
}
