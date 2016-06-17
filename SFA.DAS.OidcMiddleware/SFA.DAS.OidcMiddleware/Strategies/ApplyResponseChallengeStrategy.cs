using System;
using System.Security.Claims;
using Microsoft.Owin;
using SFA.DAS.OidcMiddleware.Builders;

namespace SFA.DAS.OidcMiddleware.Strategies
{
    public class ApplyResponseChallengeStrategy : IApplyResponseChallengeStrategy
    {
        private readonly OidcMiddlewareOptions _options;
        private readonly IBuildAuthorizeRequestUrl _buildAuthorizeRequestUrl;

        public ApplyResponseChallengeStrategy(OidcMiddlewareOptions options, IBuildAuthorizeRequestUrl buildAuthorizeRequestUrl)
        {
            _options = options;
            _buildAuthorizeRequestUrl = buildAuthorizeRequestUrl;
        }

        public void ApplyResponseChallenge(IOwinContext context)
        {
            if (context.Response.StatusCode != 401)
            {
                return;
            }

            var state = Guid.NewGuid().ToString("N");
            var nonce = Guid.NewGuid().ToString("N");

            SetTempState(state, nonce, context);

            var url = _buildAuthorizeRequestUrl.GetAuthorizeRequestUrl(_options.AuthorizeEndpoint, context.Request.Uri, _options.ClientId, _options.Scopes, state, nonce);
            context.Response.Redirect(url);
        }

        private void SetTempState(string state, string nonce , IOwinContext context)
        {
            var tempId = new ClaimsIdentity("TempState");
            tempId.AddClaim(new Claim("state", state));
            tempId.AddClaim(new Claim("nonce", nonce));

            context.Authentication.SignIn(tempId);
        }
    }
}