using System.Threading.Tasks;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.Syndication
{
    public class SyndicationMessagePublisher: IMessagePublisher
    {
        private readonly IMessageRepository _messageRepository;

        public SyndicationMessagePublisher(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task PublishAsync(object message)
        {
            await _messageRepository.StoreAsync(message);
        }
    }
}
