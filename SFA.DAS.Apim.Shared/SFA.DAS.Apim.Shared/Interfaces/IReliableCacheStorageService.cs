using System;
using System.Threading.Tasks;
using SFA.DAS.Apim.Shared.Models;

namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface IReliableCacheStorageService
    {
        Task<T> GetData<T>(IGetApiRequest request, string cacheKey, Func<ApiResponse<T>, bool> requestCheck);
    }
}