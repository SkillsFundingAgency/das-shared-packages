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

            ticket.Properties.Items["custom.session-id"] = key;

            var data = TicketSerializer.Default.Serialize(ticket);

            await _distributedCache.SetAsync(key, data, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(_configuration.LoginSlidingExpiryTimeOutInMinutes)
            });

            return key;
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            ticket.Properties.Items["custom.session-id"] = key;

            var data = TicketSerializer.Default.Serialize(ticket);

            await _distributedCache.SetAsync(key, data, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(_configuration.LoginSlidingExpiryTimeOutInMinutes)
            });
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var result = await _distributedCache.GetAsync(key);
            if (result == null) return null;

            var ticket = TicketSerializer.Default.Deserialize(result);
            ticket.Properties.Items["custom.session-id"] = key; 

            return ticket;
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}