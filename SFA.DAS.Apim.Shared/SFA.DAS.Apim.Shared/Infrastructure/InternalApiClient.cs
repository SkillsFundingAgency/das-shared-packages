using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.Apim.Shared.Interfaces;

namespace SFA.DAS.Apim.Shared.Infrastructure
{
    public class InternalApiClient<T> : ApiClient<T>, IInternalApiClient<T> where T : IInternalApiConfiguration
    {
        private readonly IAzureClientCredentialHelper _azureClientCredentialHelper;

        public InternalApiClient(
            IHttpClientFactory httpClientFactory,
            T apiConfiguration,
            IAzureClientCredentialHelper azureClientCredentialHelper) : base(httpClientFactory, apiConfiguration)
        {
            _azureClientCredentialHelper = azureClientCredentialHelper;
        }

        protected override async Task AddAuthenticationHeader(HttpRequestMessage httpRequestMessage)
        {
            if (!string.IsNullOrEmpty(Configuration.Identifier))
            {
                var accessToken = await _azureClientCredentialHelper.GetAccessTokenAsync(Configuration.Identifier);
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
    }
}
