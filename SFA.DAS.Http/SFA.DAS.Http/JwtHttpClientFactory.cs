using System.Net.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.Http
{
    public class JwtHttpClientFactory : IHttpClientFactory
    {
        private readonly IJwtClientConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public JwtHttpClientFactory(IJwtClientConfiguration configuration)
            : this(configuration, null)
        {
        }
        
        public JwtHttpClientFactory(IJwtClientConfiguration configuration, ILoggerFactory loggerFactory)
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
                .WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(_configuration))
                .Build();

            return httpClient;
        }
    }
}