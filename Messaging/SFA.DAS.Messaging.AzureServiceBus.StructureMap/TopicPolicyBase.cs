using System;
using System.Configuration;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using StructureMap;

namespace SFA.DAS.Messaging.AzureServiceBus.StructureMap
{
    public abstract class TopicPolicyBase<T> : ConfiguredInstancePolicy where T : ITopicConfiguration
    {
        protected readonly string ServiceName;

        protected TopicPolicyBase(string serviceName)
        {
            ServiceName = serviceName;
        }

        protected string GetEnvironmentName()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }
            return environment;
        }

        protected string GetMessageQueueConnectionString(string environment)
        {
            var configurationService = new ConfigurationService(GetConfigurationRepository(),
                new ConfigurationOptions(ServiceName, environment, "1.0"));

            var config = configurationService.Get<T>();

            var messageQueueConnectionString = config.MessageServiceBusConnectionString;
            return messageQueueConnectionString;
        }

        protected IConfigurationRepository GetConfigurationRepository()
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
