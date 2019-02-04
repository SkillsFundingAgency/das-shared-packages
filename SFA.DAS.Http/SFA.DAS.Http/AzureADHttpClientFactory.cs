using System;
using System.Net.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.Http
{
    public class AzureADHttpClientFactory : IHttpClientFactory
    {
        private readonly AzureActiveDirectoryClientConfiguration _configuration;

        public AzureADHttpClientFactory(AzureActiveDirectoryClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClientBuilder()
                .WithDefaultHeaders()
                .WithBearerAuthorisationHeader(new AzureADBearerTokenGenerator(_configuration))
                .Build();
            
            httpClient.BaseAddress = new Uri(_configuration.ApiBaseUrl);

            return httpClient;
        }
    }
}