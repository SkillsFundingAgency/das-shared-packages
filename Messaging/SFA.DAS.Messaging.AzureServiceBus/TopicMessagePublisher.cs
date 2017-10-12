using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Messaging.Helper;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class TopicMessagePublisher : IMessagePublisher
    {
        private readonly string _connectionString;

        public TopicMessagePublisher(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task PublishAsync(object message)
        {
            var messageGroupName = MessageGroupHelper.GetMessageGroupName(message);

            TopicClient client = null;

            try
            {
                client = TopicClient.CreateFromConnectionString(_connectionString, messageGroupName);
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
