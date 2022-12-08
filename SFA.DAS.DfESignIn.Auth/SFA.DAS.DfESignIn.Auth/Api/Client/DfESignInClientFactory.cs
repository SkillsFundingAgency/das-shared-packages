using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Api.Client
{
    public class DfESignInClientFactory : IDfESignInClientFactory, IDisposable
    {
        private readonly DfEOidcConfiguration _config;
        private readonly HttpClient _httpClient;

        private readonly ITokenDataSerializer _tokenDataSerializer;
        private readonly ITokenEncoder _tokenEncoder;
        private readonly IJsonWebAlgorithm _jsonWebAlgorithm;
        private readonly ITokenData _tokenData;

        private DfESignInClient _dfEClient;

        public DfESignInClientFactory(DfEOidcConfiguration config, HttpClient httpClient, ITokenDataSerializer tokenDataSerializer, ITokenEncoder tokenEncoder, IJsonWebAlgorithm jsonWebAlgorithm, ITokenData tokenData)
        {
            _config = config;
            _tokenDataSerializer = tokenDataSerializer;
            _tokenEncoder = tokenEncoder;
            _jsonWebAlgorithm = jsonWebAlgorithm;
            _tokenData = tokenData;
            _httpClient = httpClient;
        }

        protected string CreateDfEToken()
        {
            _tokenData.Header.Add("typ", "JWT");

            return new TokenBuilder(_tokenDataSerializer, _tokenData, _tokenEncoder, _jsonWebAlgorithm)
                .UseAlgorithm("HMACSHA256")
                .ForAudience("signin.education.gov.uk")
                .WithSecretKey(_config.APIServiceSecret)
                .Issuer(_config.ClientId)
                .CreateToken();
        }

        public async Task<HttpResponseMessage> Request(string userId = "", string organisationId = "")
        {
            _dfEClient = new DfESignInClient(_httpClient)
            {
                ServiceId = _config.APIServiceId,
                ServiceUrl = _config.APIServiceUrl,
                UserId = userId,
                OrganisationId = organisationId
            };

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, _dfEClient.TargetAddress))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CreateDfEToken());
                return await _dfEClient.HttpClient.SendAsync(requestMessage);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            _dfEClient.Dispose();
            _tokenData.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
