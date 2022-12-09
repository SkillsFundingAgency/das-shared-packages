using Microsoft.Extensions.Configuration;
using SFA.DAS.DfESignIn.SampleSite.Framework;
using SFA.DAS.DfESignIn.SampleSite.Framework.Api.Models;

using System.Net.Http;
using System.Net.Http.Headers;

namespace SFA.DAS.DfESignIn.SampleSite.Framework.Api
{
    public class DfESignInClientFactory
    {
        private readonly IConfiguration _config;

        public DfESignInClientFactory(IConfiguration config)
        {
            _config = config;
        }

        public DfESignInClient CreateDfESignInClient(string userId = "", string organizationId = "")
        {
            var dfeSignInClient = new DfESignInClient(new HttpClient())
            {
                ServiceId = _config["DfEOidcConfiguration:APIServiceId"],
                ServiceUrl = _config["DfEOidcConfiguration:APIServiceUrl"],
                UserId = userId,
                OrganisationId = organizationId
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
