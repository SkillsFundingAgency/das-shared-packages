using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Http.Configuration;
using System.Threading.Tasks;

namespace SFA.DAS.Http.TokenGenerators
{
    public class ManagedIdentityTokenGenerator : IManagedIdentityTokenGenerator
    {
        private readonly IManagedIdentityClientConfiguration _config;
        public ManagedIdentityTokenGenerator(IManagedIdentityClientConfiguration config)
        {
            _config = config;
        }

        public Task<string> Generate()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            return azureServiceTokenProvider.GetAccessTokenAsync(_config.IdentifierUri);
        }
    }
}
