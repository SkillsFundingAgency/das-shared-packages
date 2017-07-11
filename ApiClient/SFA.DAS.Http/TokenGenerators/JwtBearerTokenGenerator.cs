using System.Threading.Tasks;

namespace SFA.DAS.Http.TokenGenerators
{
    public class JwtBearerTokenGenerator : IGenerateBearerToken
    {
        private string _jwtToken;

        public JwtBearerTokenGenerator(IApiClientConfiguration configuration)
        {
            _jwtToken = configuration.ClientToken;
        }

        public Task<string> Generate()
        {
            return Task.FromResult(_jwtToken);
        }
    }
}
