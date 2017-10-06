using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class TopicMessageSubscriber<T> : IMessageSubscriber<T> where T: new()
    {
        private readonly SubscriptionClient _client;

        public TopicMessageSubscriber(string connectionString, string topicName, string subscriptionName)
        {
            _client = SubscriptionClient.CreateFromConnectionString(connectionString, topicName, subscriptionName);
        }

        public  async Task<IMessage<T>> ReceiveAsAsync()
        {
            var brokeredMessage = await _client.ReceiveAsync();

            return new AzureServiceBusMessage<T>(brokeredMessage);
        }

        public async Task<IEnumerable<IMessage<T>>> ReceiveBatchAsAsync(int batchSize)
        {
            var brokerMessages = await _client.ReceiveBatchAsync(batchSize);

            return brokerMessages.Select(msg => new AzureServiceBusMessage<T>(msg)).ToArray();
        }

        public void Dispose()
        {
            if (_client != null && !_client.IsClosed)
            {
                _client.Close();
            }
        }
    }
}
