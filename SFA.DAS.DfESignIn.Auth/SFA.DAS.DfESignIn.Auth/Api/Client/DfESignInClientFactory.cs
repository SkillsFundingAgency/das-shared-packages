using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SFA.DAS.DfESignIn.Auth.Api.Client
{
    public class DfESignInClientFactory : IDfESignInClientFactory
    {
        private readonly DfEOidcConfiguration _config;
        private readonly HttpClient _httpClient;

        private readonly ITokenDataSerializer _tokenDataSerializer;
        private readonly ITokenEncoder _tokenEncoder;
        private readonly IJsonWebAlgorithm _jsonWebAlgorithm;
        private readonly ITokenData _tokenData;

        public DfESignInClientFactory(DfEOidcConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _tokenDataSerializer = new TokenDataSerializer();
            _tokenEncoder = new TokenEncoder();
            _jsonWebAlgorithm = new JsonWebAlgorithm();
            _tokenData = new TokenData();
            _httpClient = httpClient;
        }

        public DfESignInClient CreateDfESignInClient(string userId = "", string organisationId = "")
        {
            var dfeSignInClient = new DfESignInClient(_httpClient)
            {
                ServiceId = _config.APIServiceId,
                ServiceUrl = _config.APIServiceUrl,
                UserId = userId,
                OrganisationId = organisationId
            };

            _tokenData.Header.Add("typ", "JWT");
            var token = new TokenBuilder(_tokenDataSerializer, _tokenData, _tokenEncoder, _jsonWebAlgorithm)
                .UseAlgorithm("HMACSHA256")
                .ForAudience("signin.education.gov.uk")
                .WithSecretKey(_config.APIServiceSecret)
                .Issuer(_config.ClientId)
                .CreateToken();

            dfeSignInClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return dfeSignInClient;
        }
    }
}
