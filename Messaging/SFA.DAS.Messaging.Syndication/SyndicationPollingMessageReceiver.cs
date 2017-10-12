using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.Syndication
{
    public class SyndicationPollingMessageReceiver<T> : IMessageSubscriber<T> where T : new()
    {
        private readonly IMessageClient _messageClient;
        private readonly IFeedPositionRepository _feedPositionRepository;

        public SyndicationPollingMessageReceiver(IMessageClient messageClient, IFeedPositionRepository feedPositionRepository)
        {
            _messageClient = messageClient;
            _feedPositionRepository = feedPositionRepository;
        }

        public async Task<IMessage<T>> ReceiveAsAsync() 
        {
            var clientMessage = await _messageClient.GetNextUnseenMessage<T>();
            if (clientMessage == null)
            {
                return null;
            }

            return new SyndicationMessage<T>(clientMessage.Message, clientMessage.Identifier, _feedPositionRepository);
        }
        public async Task<IEnumerable<IMessage<T>>> ReceiveBatchAsAsync(int batchSize) 
        {
            var batch = (await _messageClient.GetBatchOfUnseenMessages<T>(batchSize))?.ToArray();
            if (batch == null || !batch.Any())
            {
                return new Message<T>[0];
            }

            return batch.Select(cm => new SyndicationMessage<T>(cm.Message, cm.Identifier, _feedPositionRepository));
        }

        public void Dispose()
        {
            //No clean up required
        }
    }
}
