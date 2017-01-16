using System.Threading.Tasks;

namespace SFA.DAS.Events.Api.Client
{
    public interface ISecureHttpClient
    {
        Task<string> GetAsync(string url, string clientToken);
        Task<string> PostAsync(string url, string data, string clientToken);
        Task<string> PutAsync(string url, string data, string clientToken);
        Task<string> PatchAsync(string url, string data, string clientToken);
    }
}
