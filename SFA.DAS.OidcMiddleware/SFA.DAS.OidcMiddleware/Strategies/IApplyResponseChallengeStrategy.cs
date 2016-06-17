using Microsoft.Owin;

namespace SFA.DAS.OidcMiddleware.Strategies
{
    public interface IApplyResponseChallengeStrategy
    {
        void ApplyResponseChallenge(IOwinContext context);
    }
}