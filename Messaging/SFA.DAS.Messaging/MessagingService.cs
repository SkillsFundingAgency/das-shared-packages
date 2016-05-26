using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Messaging
{
    public class MessagingService
    {
        private readonly IMessageSubSystem _messageSubSystem;

        public MessagingService(IMessageSubSystem messageSubSystem)
        {
            _messageSubSystem = messageSubSystem;
        }

        public async Task PublishAsync<T>(T @event)
            where T : new()
        {
            if (@event == null)
            {
                await Task.FromResult<object>(null);
                return;
            }

            var message = JsonConvert.SerializeObject(@event);

            await _messageSubSystem.EnqueueAsync(message);
        }

        public async Task<Message<T>> ReceiveAsync<T>()
            where T : new()
        {
            var message = await _messageSubSystem.DequeueAsync();
            return new Message<T>(message);
        }
    }
}
