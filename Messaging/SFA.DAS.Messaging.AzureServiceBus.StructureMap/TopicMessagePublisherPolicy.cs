using System;
using System.Linq;
using System.Reflection;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using StructureMap.Pipeline;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging.AzureServiceBus.StructureMap
{
    public class TopicMessagePublisherPolicy<T> : TopicPolicyBase<T> where T : ITopicMessagePublisherConfiguration
    {
        private readonly string _serviceName;
        private readonly ILog _logger;

        public TopicMessagePublisherPolicy(string serviceName, ILog logger) : base(serviceName)
        {
            _serviceName = serviceName;
            _logger = logger;
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
                
                var publisher = new TopicMessagePublisher(messageQueueConnectionString, _logger);
                instance.Dependencies.AddForConstructorParameter(messagePublisher, publisher);
            }
        }

        private static ParameterInfo GetMessagePublisherParameter(IConfiguredInstance instance)
        {
            var messagePublisher = instance?.Constructor?
                .GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IMessagePublisher));
            return messagePublisher;
        }
    }
}