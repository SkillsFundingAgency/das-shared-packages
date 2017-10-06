using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace SFA.DAS.Messaging.AzureStorageQueue
{
    public class QueueMessageSubscriber<T> : IMessageSubscriber<T> where T : new()
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public QueueMessageSubscriber(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public async Task<IMessage<T>> ReceiveAsAsync()
        {
            var queue = GetQueue();
            var message = await queue.GetMessageAsync();

            if (message == null)
            {
                return null;
            }

            return new AzureStorageQueueMessage<T>(message, queue);
        }

        public async Task<IEnumerable<IMessage<T>>> ReceiveBatchAsAsync(int batchSize)
        {
            var queue = GetQueue();
            var batch = await queue.GetMessagesAsync(batchSize);
            return batch.Select(m => new AzureStorageQueueMessage<T>(m, queue));
        }

        public void Dispose()
        {
            //No clean up needed
        }

        private CloudQueue GetQueue()
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            return queueClient.GetQueueReference(_queueName);
        }
    }
}
