using Azure.Core;
using Azure.Identity;
using SFA.DAS.GovUK.Auth.Interfaces;

namespace SFA.DAS.GovUK.Auth.Services;

internal class AzureIdentityService : IAzureIdentityService
{
    public Task<string> AuthenticationCallback(string authority, string resource, string scope)
    {
        var azureResource = "https://vault.azure.net/.default";
        var chainedTokenCredential = new ChainedTokenCredential(
            new ManagedIdentityCredential(),
            new AzureCliCredential());
        
        var token = chainedTokenCredential.GetTokenAsync(
            new TokenRequestContext(scopes: new [] { azureResource })).Result;
        
        return Task.FromResult(token.Token);
    }
}