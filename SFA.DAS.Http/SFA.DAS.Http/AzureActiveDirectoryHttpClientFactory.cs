using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.Http
{
    public class AzureActiveDirectoryHttpClientFactory : IHttpClientFactory
    {
        private readonly IAzureActiveDirectoryClientConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public AzureActiveDirectoryHttpClientFactory(IAzureActiveDirectoryClientConfiguration configuration)
            : this(configuration, null)
        {
        }

        public AzureActiveDirectoryHttpClientFactory(IAzureActiveDirectoryClientConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClientBuilder = new HttpClientBuilder();

            if (_loggerFactory != null)
            {
                httpClientBuilder.WithLogging(_loggerFactory);
            }
                
            var httpClient = httpClientBuilder
                .WithDefaultHeaders()
                .WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(_configuration))
                .Build();
            
            httpClient.BaseAddress = new Uri(_configuration.ApiBaseUrl);

            return httpClient;
        }
    }
}