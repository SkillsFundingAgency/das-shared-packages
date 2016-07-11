using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Syndication
{
    public class SyndicationMessage<T> : Message<T>
    {
        private readonly string _idenfifier;
        private readonly IFeedPositionRepository _feedPositionRepository;

        public SyndicationMessage(T content, string idenfifier, IFeedPositionRepository feedPositionRepository) : base(content)
        {
            _idenfifier = idenfifier;
            _feedPositionRepository = feedPositionRepository;
        }


        public override async Task CompleteAsync()
        {
            await _feedPositionRepository.UpdateLastSeenMessageIdentifierAsync(_idenfifier);
        }

        public override Task AbortAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
