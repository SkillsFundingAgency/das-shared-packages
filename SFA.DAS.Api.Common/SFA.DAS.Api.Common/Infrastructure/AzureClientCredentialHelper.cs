using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using SFA.DAS.Api.Common.Interfaces;

namespace SFA.DAS.Api.Common.Infrastructure
{
    public class AzureClientCredentialHelper : IAzureClientCredentialHelper
    {
        public async Task<string> GetAccessTokenAsync(string identifier)
        {
            var azureServiceTokenProvider = new ChainedTokenCredential(
                new ManagedIdentityCredential(options: new TokenCredentialOptions
                {
                    Retry = { NetworkTimeout = TimeSpan.FromSeconds(1), MaxRetries = 2, Delay = TimeSpan.FromMilliseconds(100) }
                }),
                new AzureCliCredential());
            
            var accessToken = await azureServiceTokenProvider.GetTokenAsync(new TokenRequestContext(scopes: new[] { identifier }));

            return accessToken.Token;
        }
    }
}