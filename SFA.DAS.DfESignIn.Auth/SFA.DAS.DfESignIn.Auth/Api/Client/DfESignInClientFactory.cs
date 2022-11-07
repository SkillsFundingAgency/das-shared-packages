using Microsoft.Extensions.Configuration;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Configuration;
using System.Net.Http;

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
            var dsiConfiguration = _config.GetSection("DfEOidcConfiguration").Get<DfEOidcConfiguration>();

            var dfeSignInClient = new DfESignInClient(new HttpClient())
            {
                ServiceId = dsiConfiguration.APIServiceId,
                ServiceUrl = dsiConfiguration.APIServiceUrl,
                UserId = userId,
                OrganisationId = organisationId
            };

            var tokenData = new TokenData();
            tokenData.Header.Add("typ", "JWT");
            var token = new TokenBuilder(new TokenDataSerializer(), tokenData, new TokenEncoder(), new JsonWebAlgorithm())
                .UseAlgorithm("HMACSHA256")
                .ForAudience("signin.education.gov.uk")
                .WithSecretKey(dsiConfiguration.APIServiceSecret)
                .Issuer(dsiConfiguration.ClientId)
                .CreateToken();

            dfeSignInClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            return dfeSignInClient;
        }
    }
}
