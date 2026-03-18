using System;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.SharedOuterApi.Interfaces;
using SFA.DAS.SharedOuterApi.Models;

namespace SFA.DAS.SharedOuterApi.Infrastructure.Services
{
    public class ReliableCacheStorageService<TConfiguration> : IReliableCacheStorageService where TConfiguration : IApiConfiguration
    {
        private readonly IGetApiClient<TConfiguration> _client;
        private readonly ICacheStorageService _cacheStorageService;

        public ReliableCacheStorageService (IGetApiClient<TConfiguration> client, ICacheStorageService cacheStorageService)
        {
            _client = client;
            _cacheStorageService = cacheStorageService;
        }
        public async Task<T> GetData<T>(IGetApiRequest request, string cacheKey, Func<ApiResponse<T>, bool> requestCheck)
        {
            var cachedItem = await _cacheStorageService.RetrieveFromCache<T>(cacheKey);

            if (cachedItem != null)
            {
                return cachedItem;
            }
            
            var requestData = await _client.GetWithResponseCode<T>(request);

            if (requestData.StatusCode == HttpStatusCode.NotFound || !requestCheck(requestData))
            {
                return default;
            }
            
            if ((int)requestData.StatusCode < 200 || (int)requestData.StatusCode > 300)
            {
                var longCachedItem = await _cacheStorageService.RetrieveFromCache<T>($"{cacheKey}_extended");
                await _cacheStorageService.SaveToCache(cacheKey, longCachedItem, TimeSpan.FromMinutes(5));
                return longCachedItem;
            }

            var shortCacheTask = _cacheStorageService.SaveToCache(cacheKey, requestData.Body, TimeSpan.FromMinutes(5));
            var longCacheTask =  _cacheStorageService.SaveToCache($"{cacheKey}_extended", requestData.Body, TimeSpan.FromDays(180));

            await Task.WhenAll(shortCacheTask, longCacheTask);
            
            return requestData.Body;
        }
    }
}