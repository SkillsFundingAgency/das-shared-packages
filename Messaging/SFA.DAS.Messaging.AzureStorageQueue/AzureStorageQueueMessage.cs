using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.AzureStorageQueue
{
    public class AzureStorageQueueMessage<T> : IMessage<T>
    {
        private readonly CloudQueueMessage _message;
        private readonly CloudQueue _queue;

        public AzureStorageQueueMessage(CloudQueueMessage message, CloudQueue queue)
        {
            _message = message;
            _queue = queue;
            Content = JsonConvert.DeserializeObject<T>(message.AsString);
            Id = message.Id;
        }

        public T Content { get; protected set; }
        public string Id { get; }

        public async Task CompleteAsync()
        {
            await _queue.DeleteMessageAsync(_message);
        }

        public Task AbortAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
