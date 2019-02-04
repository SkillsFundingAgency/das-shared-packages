using System.Threading.Tasks;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.Http.TokenGenerators
{
    public class JwtBearerTokenGenerator : IGenerateBearerToken
    {
        private string _jwtToken;

        public JwtBearerTokenGenerator(IJwtClientConfiguration configuration)
        {
            _jwtToken = configuration.ClientToken;
        }

        public Task<string> Generate()
        {
            return Task.FromResult(_jwtToken);
        }
    }
}
