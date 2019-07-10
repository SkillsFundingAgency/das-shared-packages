using System;
using System.Threading.Tasks;

namespace SFA.DAS.Caches
{
    public interface IDistributedCache
    {
        Task<T> GetCustomValueAsync<T>(string key);
        Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter);
        Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter, TimeSpan maxCacheTime);
        Task<bool> ExistsAsync(string key);
        Task RemoveFromCache(string key);
        Task SetCustomValueAsync<T>(string key, T customType);
        Task SetCustomValueAsync<T>(string key, T customType, TimeSpan maxCacheTime);
    }
}