using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SFA.DAS.SharedOuterApi.Interfaces;

namespace SFA.DAS.SharedOuterApi.Infrastructure.Services
{
    public class CacheStorageService : ICacheStorageService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;
        
        public CacheStorageService (IDistributedCache distributedCache, IConfiguration configuration)
        {
            _distributedCache = distributedCache;
            _configuration = configuration;
        }
        
        public async Task<T> RetrieveFromCache<T>(string key)
        {
            var json = await _distributedCache.GetStringAsync(GetCacheKey(key));
            return json == null ? default : JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions{});
        }

        public async Task SaveToCache<T>(string key, T item, int expirationInHours, string? registryName = null)
        {
            await SaveToCache(key, item, TimeSpan.FromHours(expirationInHours), registryName);
        }

        public async Task SaveToCache<T>(string key, T item, TimeSpan expiryTimeFromNow, string? registryName = null)
        {
            var json = JsonSerializer.Serialize(item);

            await _distributedCache.SetStringAsync(GetCacheKey(key), json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiryTimeFromNow
            });

            if (!string.IsNullOrEmpty(registryName))
            {
                await AddToCacheKeyRegistry(registryName, key);
            }
        }

        public async Task DeleteFromCache(string key)
        {
            await _distributedCache.RemoveAsync(GetCacheKey(key));
        }

        private async Task AddToCacheKeyRegistry(string registryName, string key)
        {
            var registryKey = GetCacheKey(registryName);
            var keys = await RetrieveFromCache<List<string>>(registryName) ?? [];

            key = GetCacheKey(key);

            if (!keys.Contains(key))
            {
                keys.Add(key);
                var updatedJson = JsonSerializer.Serialize(keys);
                await _distributedCache.SetStringAsync(registryKey, updatedJson, new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromDays(7)
                });
            }
        }

        public async Task<List<string>> GetCacheKeyRegistry(string registryName)
        {
            var cacheKey = GetCacheKey(registryName);
            var json = await _distributedCache.GetStringAsync(cacheKey);

            return string.IsNullOrEmpty(json)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(json, new JsonSerializerOptions());
        }

        private string GetCacheKey(string keyName)
        {
            return $"{_configuration["ConfigNames"].Split(",")[0]}_{keyName}";
        }
    }
}
