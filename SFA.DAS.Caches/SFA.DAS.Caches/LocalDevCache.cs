using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Caches
{
    public class LocalDevCache : IDistributedCache
    {
        private static readonly Lazy<MemoryCache> DevCache = new Lazy<MemoryCache>(() => new MemoryCache("LocalDev"));

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(DevCache.Value.Contains(key));
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter)
        {
            return GetOrAddAsync(key, getter, Constants.DefaultCacheTime);
        }

        public Task<T> GetCustomValueAsync<T>(string key)
        {
            var cachedValue = (string)DevCache.Value[key];
            return Task.FromResult(JsonConvert.DeserializeObject<T>(cachedValue));
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter, TimeSpan maxCacheTime)
        {
            T result;

            if (DevCache.Value.Contains(key))
            {
                var existingValue = (string)DevCache.Value[key];
                result = JsonConvert.DeserializeObject<T>(existingValue);
            }
            else
            {
                result = await getter(key).ConfigureAwait(false);
                await SetCustomValueAsync(key, result, maxCacheTime);
            }

            return result;
        }

        public Task RemoveFromCache(string key)
        {
            DevCache.Value.Remove(key);
            return Task.CompletedTask;
        }

        public Task SetCustomValueAsync<T>(string key, T customType)
        {
            return SetCustomValueAsync(key, customType, Constants.DefaultCacheTime);
        }

        public Task SetCustomValueAsync<T>(string key, T customType, TimeSpan cacheTime)
        {
            DevCache.Value.Add(key, JsonConvert.SerializeObject(customType), new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.Add(cacheTime) });
            return Task.CompletedTask;
        }
    }
}