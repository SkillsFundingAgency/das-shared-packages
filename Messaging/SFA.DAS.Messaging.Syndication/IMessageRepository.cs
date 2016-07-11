using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Syndication
{
    public interface IMessageRepository
    {
        Task StoreAsync(object message);
        Task<SyndicationPage<T>> RetreivePageAsync<T>(int page, int pageSize);
    }
}
