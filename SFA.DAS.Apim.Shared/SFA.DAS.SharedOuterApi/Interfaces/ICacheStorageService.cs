using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.SharedOuterApi.Interfaces
{
    public interface ICacheStorageService
    {
        Task<T> RetrieveFromCache<T>(string key);
        Task SaveToCache<T>(string key, T item, int expirationInHours, string? registryName = null);
        Task DeleteFromCache(string key);
        Task SaveToCache<T>(string key, T item, TimeSpan expiryTimeFromNow, string? registryName = null);
        Task<List<string>> GetCacheKeyRegistry(string registryName);
    }
}
