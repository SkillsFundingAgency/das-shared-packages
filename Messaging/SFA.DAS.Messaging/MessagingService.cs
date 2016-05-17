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

        public async Task Publish<T>(T @event)
            where T : new()
        {
            if (@event == null)
            {
                await Task.FromResult<object>(null);
                return;
            }

            var message = JsonConvert.SerializeObject(@event);

            await _messageSubSsytem.Enqueue(message);
        }

        public async Task<Message<T>> Receive<T>()
            where T : new()
        {
            var message = await _messageSubSsytem.Dequeue();
            return new Message<T>(message);
        }
    }
}
