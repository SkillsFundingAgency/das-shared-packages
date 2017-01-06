using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace SFA.DAS.Bus.Client.AzureServiceBus
{
    public class AzureServiceBusClient : IBusClient
    {
        private readonly string _connectionString;
        
        public AzureServiceBusClient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task PublishAsync<T>(T message)
        {
            var topic = GetTopic<T>();
            CreateTopicIfItDoesNotExist(topic);

            var client = TopicClient.CreateFromConnectionString(_connectionString, topic);
            var brokeredMessage = new BrokeredMessage(message)
            {
                MessageId = Guid.NewGuid().ToString()
            };

            await client.SendAsync(brokeredMessage);
        }

        private string GetTopic<T>()
        {
            return typeof(T).FullName;
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
