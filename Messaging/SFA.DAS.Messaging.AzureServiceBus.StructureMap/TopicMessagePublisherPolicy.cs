using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.Messaging.AzureServiceBus.StructureMap
{
    public class TopicMessagePublisherPolicy<T> : ConfiguredInstancePolicy where T : ITopicMessagePublisherConfiguration
    {
        private readonly string _serviceName;

        public TopicMessagePublisherPolicy(string serviceName)
        {
            _serviceName = serviceName;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var messagePublisher = GetMessagePublisherParameter(instance);

            if (messagePublisher == null) return;

            var environment = GetEnvironmentName();

            var messageQueueConnectionString = GetMessageQueueConnectionString(environment);

            if (string.IsNullOrEmpty(messageQueueConnectionString))
            {
                var groupFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"/{_serviceName}/";

                var publisher = new FileSystemMessagePublisher(groupFolder);
                instance.Dependencies.AddForConstructorParameter(messagePublisher, publisher);
            }
            else
            {
                var publisher = new TopicMessagePublisher(messageQueueConnectionString);
                instance.Dependencies.AddForConstructorParameter(messagePublisher, publisher);
            }
        }

        private static ParameterInfo GetMessagePublisherParameter(IConfiguredInstance instance)
        {
            var messagePublisher = instance?.Constructor?
                .GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IMessagePublisher));
            return messagePublisher;
        }

        private static string GetEnvironmentName()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }
            return environment;
        }

        private string GetMessageQueueConnectionString(string environment)
        {
            var configurationService = new ConfigurationService(GetConfigurationRepository(),
                new ConfigurationOptions(_serviceName, environment, "1.0"));

            var config = configurationService.Get<T>();

            var messageQueueConnectionString = config.MessageServiceBusConnectionString;
            return messageQueueConnectionString;
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