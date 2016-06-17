using System;
using Microsoft.Owin.Security;
using Owin;

namespace SFA.DAS.OidcMiddleware
{
    public static class CodeFlowAuthenticationExtensions
    {
        public static IAppBuilder UseCodeFlowAuthentication(this IAppBuilder app,
            OidcMiddlewareOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            app.Use(typeof(CodeFlowAuthenticationMiddleware), app, options);

            return app;
        }

        public static IAppBuilder UseCodeFlowAuthentication(this IAppBuilder app, string clientId, string clientSecret)
        {
            return app.UseCodeFlowAuthentication(new OidcMiddlewareOptions
            {
                BaseUrl = "",
                ClientId = clientId,
                ClientSecret = clientSecret,
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = "code",
                Description = new AuthenticationDescription()
            });
        }
    }
}
