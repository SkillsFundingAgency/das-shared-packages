using Microsoft.Extensions.Configuration;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SFA.DAS.DfESignIn.Auth.Api.Client
{
    public class DfESignInClientFactory
    {
        private readonly IConfiguration _config;

        public DfESignInClientFactory(IConfiguration config)
        {
            _config = config;
        }

        public DfESignInClient CreateDfESignInClient(string userId = "", string organisationId = "")
        {
            var dfeSignInClient = new DfESignInClient(new HttpClient())
            {
                ServiceId = _config["DfEOidcConfiguration:APIServiceId"],
                ServiceUrl = _config["DfEOidcConfiguration:APIServiceUrl"],
                UserId = userId,
                OrganisationId = organisationId
            };

            var tokenData = new TokenData();
            tokenData.Header.Add("typ", "JWT");
            var token = new TokenBuilder(new TokenDataSerializer(), tokenData, new TokenEncoder(), new JsonWebAlgorithm())
                .UseAlgorithm("HMACSHA256")
                .ForAudience("signin.education.gov.uk")
                .WithSecretKey(_config["DfEOidcConfiguration:APIServiceSecret"])
                .Issuer(_config["DfEOidcConfiguration:ClientId"])
                .CreateToken();

            dfeSignInClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return dfeSignInClient;
        }
    }
}
