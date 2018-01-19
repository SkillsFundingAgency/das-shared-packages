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
        protected readonly string ServiceVersion;

        protected TopicPolicyBase(string serviceName, string serviceVersion)
        {
            ServiceName = serviceName;
            ServiceVersion = serviceVersion;
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
                new ConfigurationOptions(ServiceName, environment, ServiceVersion));

            var config = configurationService.Get<T>();

            var messageQueueConnectionString = config.MessageServiceBusConnectionString;
            return messageQueueConnectionString;
        }

        protected IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"] ?? "false"))
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
