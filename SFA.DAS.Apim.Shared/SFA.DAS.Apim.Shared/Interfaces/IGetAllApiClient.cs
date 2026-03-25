using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface IGetAllApiClient<T> : IGetApiClient<T>
    {
        Task<IEnumerable<TResponse>> GetAll<TResponse>(IGetAllApiRequest request);
    }
}