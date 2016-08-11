using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Syndication
{
    public class SyndicationPollingMessageReceiver : IPollingMessageReceiver
    {
        private readonly IMessageClient _messageClient;
        private readonly IFeedPositionRepository _feedPositionRepository;

        public SyndicationPollingMessageReceiver(IMessageClient messageClient, IFeedPositionRepository feedPositionRepository)
        {
            _messageClient = messageClient;
            _feedPositionRepository = feedPositionRepository;
        }

        public async Task<Message<T>> ReceiveAsAsync<T>() where T : new()
        {
            var clientMessage = await _messageClient.GetNextUnseenMessage<T>();
            if (clientMessage == null)
            {
                return null;
            }

            return new SyndicationMessage<T>(clientMessage.Message, clientMessage.Identifier, _feedPositionRepository);
        }
        public async Task<IEnumerable<Message<T>>> ReceiveBatchAsAsync<T>(int batchSize) where T : new()
        {
            var batch = (await _messageClient.GetBatchOfUnseenMessages<T>(batchSize))?.ToArray();
            if (batch == null || !batch.Any())
            {
                return new Message<T>[0];
            }

            return batch.Select(cm => new SyndicationMessage<T>(cm.Message, cm.Identifier, _feedPositionRepository));
        }
    }
}
