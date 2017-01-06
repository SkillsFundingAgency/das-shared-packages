using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace SFA.DAS.Bus.Client.AzureServiceBus
{
    public class AzureServiceBusClient : IBusClient
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public AzureServiceBusClient(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public async Task PublishAsync(object message)
        {
            var client = QueueClient.CreateFromConnectionString(_connectionString, _queueName);
            var brokeredMessage = new BrokeredMessage(message)
            {
                MessageId = Guid.NewGuid().ToString()
            };

            await client.SendAsync(brokeredMessage);
        }
    }
}
