using System.Net;
using System.Threading.Tasks;
using SFA.DAS.SharedOuterApi.Models;

namespace SFA.DAS.SharedOuterApi.Interfaces
{
    public interface IGetApiClient<T>
    {
        Task<TResponse> Get<TResponse>(IGetApiRequest request);
        Task<HttpStatusCode> GetResponseCode(IGetApiRequest request);
        Task<ApiResponse<TResponse>> GetWithResponseCode<TResponse>(IGetApiRequest request);
    }
}
