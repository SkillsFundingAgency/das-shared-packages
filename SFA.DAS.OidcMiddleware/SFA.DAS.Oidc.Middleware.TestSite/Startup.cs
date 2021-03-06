﻿using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SFA.DAS.OidcMiddleware;

[assembly: OwinStartup(typeof(SFA.DAS.Oidc.Middleware.TestSite.Startup))]

namespace SFA.DAS.Oidc.Middleware.TestSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap= new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "TempState",
                AuthenticationMode = AuthenticationMode.Passive
            });

            app.UseCodeFlowAuthentication(new OidcMiddlewareOptions
            {
                BaseUrl = Constants.BaseAddress,
                ClientId = "[CLIENT_ID]",
                ClientSecret = "[CLIENT_SECRET]",
                Scopes = "openid",
                AuthorizeEndpoint = Constants.AuthorizeEndpoint,
                TokenEndpoint = Constants.TokenEndpoint,
                UserInfoEndpoint = Constants.UserInfoEndpoint,
                AuthenticatedCallback = identity =>
                {
                    identity.AddClaim(new Claim("CustomClaim", "new claim added"));

                    // This can be used to translate claims to known identifiers for the OTB ASP.NET MVC Template
                    /*identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identity.Claims.First(c => c.Type == "user_id").Value));
                    identity.AddClaim(new Claim(ClaimTypes.Name, identity.Claims.First(c => c.Type == "display_name").Value));*/
                },
                TokenValidationMethod = TokenValidationMethod.BinarySecret,

                // Implement this if TokenValidationMethod = TokenValidationMethod.SigningKey
                /*TokenSigningCertificateLoader = () =>
                {
                    return new X509Certificate2(@"[PATH_TO_YOUR_PFX]", "[YOUR_PFX_PASSWORD]");
                }*/
            });
        }
    }

    public static class Constants
    {
        public const string AuthorizeEndpoint = BaseAddress + "/Login/dialog/appl/oidctest/wflow/authorize";
        public const string BaseAddress = "[END_POINT_FOR_IDENTITY_URL]";
        public const string IdentityTokenValidationEndpoint = BaseAddress + "/connect/identitytokenvalidation";
        public const string LogoutEndpoint = BaseAddress + "/connect/endsession";
        public const string TokenEndpoint = BaseAddress + "/Login/rest/appl/oidctest/wflow/token";
        public const string TokenRevocationEndpoint = BaseAddress + "/connect/revocation";
        public const string UserInfoEndpoint = BaseAddress + "/Login/rest/appl/oidctest/wflow/userinfo";
    }
}