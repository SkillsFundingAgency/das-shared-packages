using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Messaging
{
    public class MessagingService
    {
        private readonly IMessageSubSystem _messageSubSsytem;

        public MessagingService(IMessageSubSystem messageSubSsytem)
        {
            _messageSubSsytem = messageSubSsytem;
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

            await _messageSubSsytem.EnqueueAsync(message);
        }

        public async Task<Message<T>> ReceiveAsync<T>()
            where T : new()
        {
            var message = await _messageSubSsytem.DequeueAsync();
            return new Message<T>(message);
        }
    }
}
