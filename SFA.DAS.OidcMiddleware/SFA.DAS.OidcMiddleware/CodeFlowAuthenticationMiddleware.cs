using Microsoft.Owin;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using SFA.DAS.OidcMiddleware.Builders;
using SFA.DAS.OidcMiddleware.Caches;
using SFA.DAS.OidcMiddleware.Clients;
using SFA.DAS.OidcMiddleware.Validators;

namespace SFA.DAS.OidcMiddleware
{
    public class CodeFlowAuthenticationMiddleware : AuthenticationMiddleware<OidcMiddlewareOptions>
    {
        public CodeFlowAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, OidcMiddlewareOptions options)
            : base(next, options)
        {
            if (Options.StateDataFormat == null)
            {
                var dataProtector = app.CreateDataProtector(
                    typeof(CodeFlowAuthenticationMiddleware).FullName,
                    Options.AuthenticationType, "v1");

                Options.StateDataFormat = new PropertiesDataFormat(dataProtector);
            }
        }

        protected override AuthenticationHandler<OidcMiddlewareOptions> CreateHandler()
        {
            return new CodeFlowAuthenticationHandler(
                Options.AuthorizeUrlBuilder ?? new AuthorizeUrlBuilder(),
                Options.NonceCache ?? new NonceCache(),
                Options.TokenClient ?? new TokenClientWrapper(),
                Options.TokenValidator ?? new TokenValidator(),
                Options.UserInfoClient ?? new UserInfoClientWrapper());
        }
    }
}
