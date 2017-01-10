using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class AzureServiceBusSubscribedMessageService : ISubscribedMessagePublisher
    {
        private readonly string _connectionString;

        public AzureServiceBusSubscribedMessageService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task PublishAsync(object message)
        {
            var topic = GetTopic(message);
            CreateTopicIfItDoesNotExist(topic);

            var client = TopicClient.CreateFromConnectionString(_connectionString, topic);
            var brokeredMessage = new BrokeredMessage(message)
            {
                MessageId = Guid.NewGuid().ToString()
            };

            await client.SendAsync(brokeredMessage);
        }

        private string GetTopic(object message)
        {
            return message.GetType().FullName;
        }

        private void CreateTopicIfItDoesNotExist(string topic)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);

            if (!namespaceManager.TopicExists(topic))
            {
                namespaceManager.CreateTopic(topic);
            }
        }
    }
}
