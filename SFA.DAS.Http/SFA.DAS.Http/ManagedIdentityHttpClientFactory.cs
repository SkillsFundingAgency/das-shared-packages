using Microsoft.Extensions.Logging;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;
using System;
using System.Net.Http;

namespace SFA.DAS.Http
{
    public class ManagedIdentityHttpClientFactory : IHttpClientFactory
    {
        private readonly IManagedIdentityClientConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public ManagedIdentityHttpClientFactory(IManagedIdentityClientConfiguration configuration)
            : this(configuration, null)
        {
        }

        public ManagedIdentityHttpClientFactory(IManagedIdentityClientConfiguration configuration, ILoggerFactory loggerFactory)
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
                .WithManagedIdentityAuthorisationHeader(new ManagedIdentityTokenGenerator(_configuration))
                .Build();

            httpClient.BaseAddress = new Uri(_configuration.ApiBaseUrl);

            return httpClient;
        }
    }
}
