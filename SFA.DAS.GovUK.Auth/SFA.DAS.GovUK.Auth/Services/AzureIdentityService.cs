using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;

namespace SFA.DAS.GovUK.Auth.Services
{
    internal class AzureIdentityService : IAzureIdentityService
    {
        public async Task<string> AuthenticationCallback(string authority, string resource, string scope)
        {
            var chainedTokenCredential = new ChainedTokenCredential(
                new ManagedIdentityCredential(options:new TokenCredentialOptions
                {
                    Retry = { NetworkTimeout = TimeSpan.FromMilliseconds(500),MaxRetries = 2, Delay = TimeSpan.FromMilliseconds(100)}
                }),
                new AzureCliCredential());

            var token = await chainedTokenCredential.GetTokenAsync(
                new TokenRequestContext(scopes: new[] {"https://vault.azure.net/.default"}));

            return token.Token;
        }
    }
}