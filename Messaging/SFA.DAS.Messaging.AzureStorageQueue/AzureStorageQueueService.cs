using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace SFA.DAS.Messaging.AzureStorageQueue
{
    public class AzureStorageQueueService : IMessagePublisher, IPollingMessageReceiver
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public AzureStorageQueueService(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public async Task PublishAsync(object message)
        {
            var content = JsonConvert.SerializeObject(message);

            var queue = GetQueue();
            await queue.AddMessageAsync(new CloudQueueMessage(content));
        }

        public async Task<Message<T>> ReceiveAsAsync<T>() where T : new()
        {
            var queue = GetQueue();
            var message = await queue.GetMessageAsync();
            if (message == null)
            {
                return null;
            }

            return new AzureStorageQueueMessage<T>(message, queue);
        }
        public async Task<IEnumerable<Message<T>>> ReceiveBatchAsAsync<T>(int batchSize) where T : new()
        {
            var queue = GetQueue();
            var batch = await queue.GetMessagesAsync(batchSize);
            return batch.Select(m => new AzureStorageQueueMessage<T>(m, queue));
        }

        private CloudQueue GetQueue()
        {

            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            return queueClient.GetQueueReference(_queueName);
        }
    }
}
