using System;
using System.Net.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.Http
{
    public class AzureActiveDirectoryHttpClientFactory : IHttpClientFactory
    {
        private readonly IAzureActiveDirectoryClientConfiguration _configuration;

        public AzureActiveDirectoryHttpClientFactory(IAzureActiveDirectoryClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClientBuilder()
                .WithDefaultHeaders()
                .WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(_configuration))
                .Build();
            
            httpClient.BaseAddress = new Uri(_configuration.ApiBaseUrl);

            return httpClient;
        }
    }
}