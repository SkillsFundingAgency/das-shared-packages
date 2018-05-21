using System;
using System.Threading.Tasks;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.Syndication
{
    public class SyndicationMessage<T> : IMessage<T>
    {
        private readonly string _idenfifier;
        private readonly IFeedPositionRepository _feedPositionRepository;

        public SyndicationMessage(T content, string idenfifier, IFeedPositionRepository feedPositionRepository)
        {
            _idenfifier = idenfifier;
            _feedPositionRepository = feedPositionRepository;
            Content = content;
            Id = Guid.NewGuid().ToString();
        }

        public T Content { get; protected set; }
        public string Id { get; }

        public async Task CompleteAsync()
        {
            await _feedPositionRepository.UpdateLastSeenMessageIdentifierAsync(_idenfifier);
        }

        public Task AbortAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
