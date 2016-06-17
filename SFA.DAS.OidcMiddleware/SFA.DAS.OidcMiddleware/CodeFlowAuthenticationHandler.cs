using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using SFA.DAS.OidcMiddleware.Strategies;

namespace SFA.DAS.OidcMiddleware
{
    public class CodeFlowAuthenticationHandler : AuthenticationHandler<OidcMiddlewareOptions>
    {
        private readonly IAuthenticateCoreStrategy _authenticateCoreStrategy;
        private readonly IApplyResponseChallengeStrategy _applyResponseChallengeStrategy;

        public CodeFlowAuthenticationHandler(IAuthenticateCoreStrategy authenticateCoreStrategy, IApplyResponseChallengeStrategy applyResponseChallengeStrategy)
        {
            _authenticateCoreStrategy = authenticateCoreStrategy;
            _applyResponseChallengeStrategy = applyResponseChallengeStrategy;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            return await _authenticateCoreStrategy.Authenticate(Context);
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            _applyResponseChallengeStrategy.ApplyResponseChallenge(Context);

            return Task.FromResult<object>(null);
        }
    }
}
