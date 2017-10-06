using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace SFA.DAS.Messaging.AzureStorageQueue
{
    public class QueueMessagePublisher<T> : IMessagePublisher<T> where T : new()
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public QueueMessagePublisher(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public async Task PublishAsync(T message)
        {
            var content = JsonConvert.SerializeObject(message);

            var queue = GetQueue();
            await queue.AddMessageAsync(new CloudQueueMessage(content));
        }

        private CloudQueue GetQueue()
        {

            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            return queueClient.GetQueueReference(_queueName);
        }
    }
}
