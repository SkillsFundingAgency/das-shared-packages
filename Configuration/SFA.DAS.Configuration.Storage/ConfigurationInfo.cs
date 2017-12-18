using System;
using System.Configuration;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;

namespace SFA.DAS.Storage
{
    public class ConfigurationInfo<T> : IConfigurationInfo<T>
    {
        private readonly CloudConfigurationSettings _cloudConfigSettings;

        public ConfigurationInfo(CloudConfigurationSettings cloudConfigSettings)
        {
            _cloudConfigSettings = cloudConfigSettings;
        }

        public T GetConfiguration(string serviceName)
        {
            return GetConfiguration(serviceName, null);
        }

        public T GetConfiguration(string serviceName, Action<string> action)
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = _cloudConfigSettings.GetSetting("EnvironmentName");
            }
            action?.Invoke(environment);

            var configurationRepository = GetConfigurationRepository();
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(serviceName, environment, "1.0"));

            var result = configurationService.Get<T>();

            return result;
        }

        private IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(_cloudConfigSettings.GetSetting("ConfigurationStorageConnectionString"));
            }
            return configurationRepository;
        }
    }
}