using System;
using Microsoft.Azure;
using System.Configuration;
using SFA.DAS.Configuration.FileStorage;

namespace SFA.DAS.Configuration.AzureTableStorage.Environment
{
    public class ConfigurationInfo<T> : IConfigurationInfo<T>
    {
        public T GetConfiguration(string serviceName)
        {
            return GetConfiguration(serviceName, null);
        }

        public T GetConfiguration(string serviceName, Action<string> action)
        {
            var environment = System.Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }
            action?.Invoke(environment);

            var configurationRepository = GetConfigurationRepository();
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(serviceName, environment, "1.0"));

            var result = configurationService.Get<T>();

            return result;
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }
            return configurationRepository;
        }
    }
}