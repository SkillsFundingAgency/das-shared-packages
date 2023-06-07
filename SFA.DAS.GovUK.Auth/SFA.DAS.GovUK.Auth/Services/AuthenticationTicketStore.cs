using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SFA.DAS.GovUK.Auth.Configuration;

namespace SFA.DAS.GovUK.Auth.Services
{
    public class AuthenticationTicketStore : ITicketStore
    {
        private readonly IDistributedCache _distributedCache;
        private readonly GovUkOidcConfiguration _configuration;

        public AuthenticationTicketStore(IDistributedCache distributedCache, IOptions<GovUkOidcConfiguration> configuration)
        {
            _distributedCache = distributedCache;
            _configuration = configuration.Value;
        }
        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = Guid.NewGuid().ToString();
            await _distributedCache.SetAsync(key,TicketSerializer.Default.Serialize(ticket), new DistributedCacheEntryOptions
            {
                SlidingExpiration =  TimeSpan.FromMinutes(_configuration.LoginSlidingExpiryTimeOutInMinutes)
            });
            return key;
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            await _distributedCache.RefreshAsync(key);
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var result = await _distributedCache.GetAsync(key);
            return result == null ? null : TicketSerializer.Default.Deserialize(result);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}