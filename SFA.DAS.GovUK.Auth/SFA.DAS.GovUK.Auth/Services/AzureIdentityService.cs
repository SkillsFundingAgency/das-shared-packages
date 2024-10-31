using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.GovUK.Auth.Services
{
    internal class AzureIdentityService : IAzureIdentityService
    {
        private readonly bool _isLocal;

        public AzureIdentityService(IConfiguration configuration)
        {
            _isLocal = configuration["ResourceEnvironmentName"]!.Equals("LOCAL", StringComparison.InvariantCultureIgnoreCase);
        }
        public async Task<string> AuthenticationCallback(string authority, string resource, string scope)
        {
            ChainedTokenCredential chainedTokenCredential;

            if (_isLocal)
            {
                chainedTokenCredential = new ChainedTokenCredential(
                    new AzureCliCredential(options: new AzureCliCredentialOptions
                    {
                        Retry = { NetworkTimeout = TimeSpan.FromMilliseconds(500),MaxRetries = 2, Delay = TimeSpan.FromMilliseconds(100)}
                    }));    
            }
            else
            {
                chainedTokenCredential = new ChainedTokenCredential(
                    new ManagedIdentityCredential(options:new TokenCredentialOptions
                    {
                        Retry = { NetworkTimeout = TimeSpan.FromMilliseconds(500),MaxRetries = 2, Delay = TimeSpan.FromMilliseconds(100)}
                    }));
            }
            

            var token = await chainedTokenCredential.GetTokenAsync(
                new TokenRequestContext(scopes: new[] {"https://vault.azure.net/.default"}));

            return token.Token;
        }
    }
}