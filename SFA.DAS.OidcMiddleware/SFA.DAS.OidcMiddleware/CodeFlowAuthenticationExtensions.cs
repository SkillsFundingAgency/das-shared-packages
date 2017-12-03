using System;
using Microsoft.Owin.Security;
using Owin;

namespace SFA.DAS.OidcMiddleware
{
    public static class CodeFlowAuthenticationExtensions
    {
        public static IAppBuilder UseCodeFlowAuthentication(this IAppBuilder app, string clientId, string clientSecret)
        {
            return app.UseCodeFlowAuthentication(new OidcMiddlewareOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = "code",
                BaseUrl = "",
                ClientId = clientId,
                ClientSecret = clientSecret,
                Description = new AuthenticationDescription()
            });
        }

        public static IAppBuilder UseCodeFlowAuthentication(this IAppBuilder app, OidcMiddlewareOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            app.Use(typeof(CodeFlowAuthenticationMiddleware), app, options);

            return app;
        }
    }
}
