using System.Threading.Tasks;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.Http.TokenGenerators
{
    public class StaticJwtBearerTokenGenerator : IGenerateBearerToken
    {
        private readonly string _jwtToken;

        public StaticJwtBearerTokenGenerator(IStaticJwtClientConfiguration configuration)
        {
            _jwtToken = configuration.ClientToken;
        }

        public Task<string> Generate()
        {
            return Task.FromResult(_jwtToken);
        }
    }
}
