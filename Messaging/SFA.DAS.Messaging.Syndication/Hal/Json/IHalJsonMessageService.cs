using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Syndication.Hal.Json
{
    public interface IHalJsonMessageService<T>
    {
        Task<HalResponse> GetPageAsync(int page);
    }
}