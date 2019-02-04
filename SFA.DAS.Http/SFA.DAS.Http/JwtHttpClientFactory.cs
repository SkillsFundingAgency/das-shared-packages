using System.Net.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.Http
{
    public class JwtHttpClientFactory : IHttpClientFactory
    {
        private readonly IJwtClientConfiguration _configuration;


        public JwtHttpClientFactory(IJwtClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClientBuilder()
                .WithDefaultHeaders()
                .WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(_configuration))
                .Build();

            return httpClient;
        }
    }
}