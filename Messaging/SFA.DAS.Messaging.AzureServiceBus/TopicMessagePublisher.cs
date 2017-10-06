using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class TopicMessagePublisher<T> : IMessagePublisher<T> where T : new()
    {
        private readonly string _connectionString;
        private readonly string _topicName;


        public TopicMessagePublisher(string connectionString, string topicName)
        {
            _connectionString = connectionString;
            _topicName = topicName;
        }

        public async Task PublishAsync(T message)
        {
            TopicClient client = null;

            try
            {
                client = TopicClient.CreateFromConnectionString(_connectionString, _topicName);
                await client.SendAsync(new BrokeredMessage(message));
            }
            finally
            {
                if (client != null && !client.IsClosed)
                {
                    await client.CloseAsync();
                }
            }
        }
    }
}
