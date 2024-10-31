using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Api.Common.Interfaces;

namespace SFA.DAS.Api.Common.Infrastructure
{
    public class AzureClientCredentialHelper : IAzureClientCredentialHelper
    {
        private const int MaxRetries = 2;
        private readonly TimeSpan _networkTimeout = TimeSpan.FromMilliseconds(500);
        private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(100);
        private readonly bool _isLocal;

        [Obsolete("Use AzureClientCredentialHelper(IConfiguration) instead")]
        public AzureClientCredentialHelper()
        {
            _isLocal = false;
        }
        
        public AzureClientCredentialHelper(IConfiguration configuration)
        {
            var resourceEnvironmentName = configuration["ResourceEnvironmentName"];
            _isLocal = resourceEnvironmentName != null && resourceEnvironmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);
        }
        
        public async Task<string> GetAccessTokenAsync(string identifier)
        {
            ChainedTokenCredential azureServiceTokenProvider;
            if (_isLocal)
            {
                azureServiceTokenProvider = new ChainedTokenCredential(
                    new AzureCliCredential(options: new AzureCliCredentialOptions
                    {
                        Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay, Mode = RetryMode.Fixed }
                    }),
                    new VisualStudioCredential(options: new VisualStudioCredentialOptions
                    {
                        Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay, Mode = RetryMode.Fixed }
                    }),
                    new VisualStudioCodeCredential(options: new VisualStudioCodeCredentialOptions
                    {
                        Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay, Mode = RetryMode.Fixed }
                    }));   
                
            }
            else
            {
                azureServiceTokenProvider = new ChainedTokenCredential(
                    new ManagedIdentityCredential(options: new TokenCredentialOptions
                    {
                        Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay, Mode = RetryMode.Fixed }
                    }),
                    new AzureCliCredential(options: new AzureCliCredentialOptions
                    {
                        Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay, Mode = RetryMode.Fixed }
                    }),
                    new VisualStudioCredential(options: new VisualStudioCredentialOptions
                    {
                        Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay, Mode = RetryMode.Fixed }
                    }),
                    new VisualStudioCodeCredential(options: new VisualStudioCodeCredentialOptions
                    {
                        Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay, Mode = RetryMode.Fixed }
                    }));
            }
            
            var accessToken = await azureServiceTokenProvider.GetTokenAsync(new TokenRequestContext(scopes: [identifier]));

            return accessToken.Token;
        }
    }
}