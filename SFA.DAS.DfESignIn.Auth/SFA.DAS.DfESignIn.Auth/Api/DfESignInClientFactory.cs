using Microsoft.Extensions.Configuration;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Models;
using System.Net.Http;

namespace SFA.DAS.DfESignIn.Auth.Api
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
            var dsiConfiguration = _config.GetSection("DfEOidcConfiguration").Get<DfEOidcConfiguration>();

            var dfeSignInClient = new DfESignInClient(new HttpClient())
            {
                ServiceId = dsiConfiguration.APIServiceId,
                ServiceUrl = dsiConfiguration.APIServiceUrl,
                UserId = userId,
                OrganisationId = organizationId
            };

            var tokenData = new TokenData();
            tokenData.Header.Add("typ", "JWT");
            var token = new TokenBuilder(new TokenDataSerializer(), tokenData, new TokenEncoder(), new JsonWebAlgorithm())
                .UseAlgorithm("HMACSHA256")
                .ForAudience(dsiConfiguration.APIServiceAudience)
                .WithSecretKey(dsiConfiguration.APIServiceSecret)
                .Issuer(dsiConfiguration.ClientId)
                .CreateToken();

            dfeSignInClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            return dfeSignInClient;
        }
    }
}
