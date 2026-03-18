using System;
using System.Threading.Tasks;
using SFA.DAS.SharedOuterApi.Models;

namespace SFA.DAS.SharedOuterApi.Interfaces
{
    public interface IReliableCacheStorageService
    {
        Task<T> GetData<T>(IGetApiRequest request, string cacheKey, Func<ApiResponse<T>, bool> requestCheck);
    }
}