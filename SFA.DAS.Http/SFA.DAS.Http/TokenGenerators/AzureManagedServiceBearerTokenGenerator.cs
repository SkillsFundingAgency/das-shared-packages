using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Http.Configuration;
using System.Threading.Tasks;

namespace SFA.DAS.Http.TokenGenerators
{
    public class AzureManagedServiceBearerTokenGenerator : IGenerateBearerToken
    {
        readonly IAzureManagedServiceClientConfiguration _config;

        public AzureManagedServiceBearerTokenGenerator(IAzureManagedServiceClientConfiguration configuration)
        {
            _config = configuration;
        }

        public Task<string> Generate()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            return azureServiceTokenProvider.GetAccessTokenAsync(_config.SubscriptionKey);
        }
    }
}