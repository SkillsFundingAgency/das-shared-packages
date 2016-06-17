using Microsoft.Owin;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using SFA.DAS.OidcMiddleware.Builders;
using SFA.DAS.OidcMiddleware.Strategies;
using SFA.DAS.OidcMiddleware.Validators;

namespace SFA.DAS.OidcMiddleware
{
    public class CodeFlowAuthenticationMiddleware : AuthenticationMiddleware<OidcMiddlewareOptions>
    {
        private readonly AuthenticateCoreStrategy _authenticateCoreStrategy;
        private readonly ApplyResponseChallengeStrategy _applyResponseChallengeStrategy;

        public CodeFlowAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, OidcMiddlewareOptions options) : base(next, options)
        {
            if (Options.StateDataFormat == null)
            {
                var dataProtector = app.CreateDataProtector(
                    typeof(CodeFlowAuthenticationMiddleware).FullName,
                    Options.AuthenticationType, "v1");
                Options.StateDataFormat = new PropertiesDataFormat(dataProtector);
            }

            _authenticateCoreStrategy = new AuthenticateCoreStrategy(
                Options,
                new BuildRequestAuthorisationCode(),
                new BuildUserInfoClientUrl(),
                new SecurityTokenValidation(Options));
            _applyResponseChallengeStrategy = new ApplyResponseChallengeStrategy(
                Options,
                new BuildAuthorizeRequestUrl());
        }

        protected override AuthenticationHandler<OidcMiddlewareOptions> CreateHandler()
        {
            
            return new CodeFlowAuthenticationHandler(_authenticateCoreStrategy, _applyResponseChallengeStrategy);
        }
    }
}
