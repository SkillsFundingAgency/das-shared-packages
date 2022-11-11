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
        private readonly HttpClient _client = new HttpClient();

        public DfESignInClientFactory(DfEOidcConfiguration config)
        {
            _config = config;
        }

        public DfESignInClient CreateDfESignInClient(string userId = "", string organisationId = "")
        {
            var dfeSignInClient = new DfESignInClient(_client)
            {
                ServiceId = _config.APIServiceId,
                ServiceUrl = _config.APIServiceUrl,
                UserId = userId,
                OrganisationId = organisationId
            };

            var tokenData = new TokenData();
            tokenData.Header.Add("typ", "JWT");
            var token = new TokenBuilder(new TokenDataSerializer(), tokenData, new TokenEncoder(), new JsonWebAlgorithm())
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
