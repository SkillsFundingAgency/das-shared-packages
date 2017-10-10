using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Syndication
{
    public class SyndicationMessagePublisher<T> : IMessagePublisher<T> where T : new()
    {
        private readonly IMessageRepository _messageRepository;

        public SyndicationMessagePublisher(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task PublishAsync(T message)
        {
            await _messageRepository.StoreAsync(message);
        }
    }
}
