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
                new ManagedIdentityCredential(),
                new AzureCliCredential());

            var token = await chainedTokenCredential.GetTokenAsync(
                new TokenRequestContext(scopes: new[] {"https://vault.azure.net/.default"}));

            return token.Token;
        }
    }
}