using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Syndication
{
    public interface IFeedPositionRepository
    {
        Task<string> GetLastSeenMessageIdentifierAsync();
        Task UpdateLastSeenMessageIdentifierAsync(string identifier);
    }
}
