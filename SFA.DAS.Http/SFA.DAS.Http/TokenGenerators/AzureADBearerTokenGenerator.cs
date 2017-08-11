using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SFA.DAS.Http.TokenGenerators
{
    public class AzureADBearerTokenGenerator : IGenerateBearerToken
    {
        IAzureADClientConfiguration _config;

        public AzureADBearerTokenGenerator(IAzureADClientConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<string> Generate()
        {
            var authority = $"https://login.microsoftonline.com/{_config.Tenant}";
            var clientCredential = new ClientCredential(_config.ClientId, _config.ClientSecret);
            var context = new AuthenticationContext(authority, true);
            var result = await context.AcquireTokenAsync(_config.IdentifierUri, clientCredential);

            return result.AccessToken;
        }
    }
}
