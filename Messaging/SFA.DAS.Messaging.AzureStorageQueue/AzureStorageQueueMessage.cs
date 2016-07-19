using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace SFA.DAS.Messaging.AzureStorageQueue
{
    public class AzureStorageQueueMessage<T> : Message<T>
    {
        private readonly CloudQueueMessage _message;
        private readonly CloudQueue _queue;

        public AzureStorageQueueMessage(CloudQueueMessage message, CloudQueue queue)
        {
            _message = message;
            _queue = queue;
            Content = JsonConvert.DeserializeObject<T>(message.AsString);
        }

        public override async Task CompleteAsync()
        {
            await _queue.DeleteMessageAsync(_message);
        }

        public override Task AbortAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
