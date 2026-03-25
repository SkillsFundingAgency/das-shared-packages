using System.Net;
using System.Threading.Tasks;
using SFA.DAS.Apim.Shared.Models;

namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface IGetApiClient<T>
    {
        Task<TResponse> Get<TResponse>(IGetApiRequest request);
        Task<HttpStatusCode> GetResponseCode(IGetApiRequest request);
        Task<ApiResponse<TResponse>> GetWithResponseCode<TResponse>(IGetApiRequest request);
    }
}
