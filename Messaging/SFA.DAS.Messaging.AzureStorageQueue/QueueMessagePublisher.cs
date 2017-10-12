using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SFA.DAS.Messaging.Helpers;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.AzureStorageQueue
{
    public class QueueMessagePublisher: IMessagePublisher
    {
        private readonly string _connectionString;


        public QueueMessagePublisher(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task PublishAsync(object message)
        {
            var messageGroupName = MessageGroupHelper.GetMessageGroupName(message);

            var content = JsonConvert.SerializeObject(message);

            var queue = GetQueue(messageGroupName);
            await queue.AddMessageAsync(new CloudQueueMessage(content));
        }

        private CloudQueue GetQueue(string queueName)
        {

            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            return queueClient.GetQueueReference(queueName);
        }
    }
}
