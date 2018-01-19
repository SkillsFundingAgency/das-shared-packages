using System;
using System.Linq;
using System.Reflection;
using SFA.DAS.Messaging.AzureServiceBus.Helpers;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using StructureMap.Pipeline;

namespace SFA.DAS.Messaging.AzureServiceBus.StructureMap
{
    public class TopicMessageSubscriberPolicy<T> : TopicPolicyBase<T> where T : ITopicMessageSubscriberConfiguration
    {
        public TopicMessageSubscriberPolicy(string serviceName, string serviceVersion) : base(serviceName, serviceVersion)
        {
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var subscriberFactory = GetMessageSubscriberFactoryParameter(instance);

            if (subscriberFactory == null) return;

            var environment = GetEnvironmentName();

            var messageQueueConnectionString = GetMessageQueueConnectionString(environment);

            if (string.IsNullOrEmpty(messageQueueConnectionString))
            {
                var groupFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/EAS_Queues/";
                var factory = new FileSystemMessageSubscriberFactory(groupFolder);

                instance.Dependencies.AddForConstructorParameter(subscriberFactory, factory);
            }
            else
            {
                var subscriptionName = TopicSubscriptionHelper.GetMessageGroupName(instance.Constructor.DeclaringType);

                var factory = new TopicSubscriberFactory(messageQueueConnectionString, subscriptionName, new NLogLogger(typeof(TopicSubscriberFactory)));

                instance.Dependencies.AddForConstructorParameter(subscriberFactory, factory);
            }
        }

        private static ParameterInfo GetMessageSubscriberFactoryParameter(IConfiguredInstance instance)
        {
            var factory = instance?.Constructor?
                .GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IMessageSubscriberFactory));

            return factory;
        }
    }
}
