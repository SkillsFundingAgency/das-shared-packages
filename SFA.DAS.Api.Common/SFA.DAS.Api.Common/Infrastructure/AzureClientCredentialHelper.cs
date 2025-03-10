using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Api.Common.Interfaces;

namespace SFA.DAS.Api.Common.Infrastructure;

public class AzureClientCredentialHelper : IAzureClientCredentialHelper
{
    private const int MaxRetries = 2;
    private readonly TimeSpan _networkTimeout = TimeSpan.FromMilliseconds(500);
    private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(100);

    // Take advantage of built-in token caching.
    // https://learn.microsoft.com/en-us/dotnet/api/azure.identity.chainedtokencredential?view=azure-dotnet
    private ChainedTokenCredential _chainedTokenCredential;

    [Obsolete("Use AzureClientCredentialHelper(IConfiguration) instead")]
    public AzureClientCredentialHelper()
    {
        InitializeChainedTokenCredential(false);
    }

    public AzureClientCredentialHelper(IConfiguration configuration)
    {
        var resourceEnvironmentName = configuration["ResourceEnvironmentName"];
        var isLocal = resourceEnvironmentName != null && resourceEnvironmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);

        InitializeChainedTokenCredential(isLocal);
    }

    private void InitializeChainedTokenCredential(bool isLocal)
    {
        if (isLocal)
        {
            _chainedTokenCredential = new ChainedTokenCredential(
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
            _chainedTokenCredential = new ChainedTokenCredential(
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
    }

    public async Task<string> GetAccessTokenAsync(string identifier)
    {
        var accessToken = await _chainedTokenCredential.GetTokenAsync(new TokenRequestContext(scopes: [identifier]));

        return accessToken.Token;
    }
}