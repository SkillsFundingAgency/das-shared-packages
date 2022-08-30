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
                new ManagedIdentityCredential(),
                new AzureCliCredential());
            var accessToken = await azureServiceTokenProvider.GetTokenAsync(new TokenRequestContext(scopes:new[]{identifier}));
         
            return accessToken.Token;
        }
    }
}