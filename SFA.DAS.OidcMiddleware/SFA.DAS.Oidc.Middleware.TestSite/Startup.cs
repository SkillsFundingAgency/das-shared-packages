using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using SFA.DAS.OidcMiddleware.GovUk.Configuration;
using SFA.DAS.OidcMiddleware.GovUk.Services;

[assembly: OwinStartup(typeof(SFA.DAS.Oidc.Middleware.TestSite.Startup))]

namespace SFA.DAS.Oidc.Middleware.TestSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap= new Dictionary<string, string>();
            app.SetDefaultSignInAsAuthenticationType(OpenIdConnectAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "TempState",
                AuthenticationMode = AuthenticationMode.Passive
            });

            var govUkOidcConfiguration = new GovUkOidcConfiguration
            {
                ClientId = "{CLIENT_ID}",
                BaseUrl = "https://{BASE_URL}/",
                KeyVaultIdentifier = "{KEY_VAULT_IDENTIFIER}"
            };
            var handler = new JwtSecurityTokenService(govUkOidcConfiguration);

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions("code")
            {
                ClientId = govUkOidcConfiguration.ClientId,
                Scope = "openid email phone",
                Authority = govUkOidcConfiguration.BaseUrl,
                MetadataAddress = $"{govUkOidcConfiguration.BaseUrl}/.well-known/openid-configuration",
                ResponseType = OpenIdConnectResponseType.Code,
                ResponseMode = "",
                SaveTokens = true,
                RedeemCode = true,
                RedirectUri = "https://localhost:44363/sign-in",
                UsePkce = false,
                CookieManager = new ChunkingCookieManager(),
                SignInAsAuthenticationType = "Cookies",
                SecurityTokenValidator = handler,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = notification =>
                    {
                        var code = notification.Code;

                        var redirectUri = notification.RedirectUri;

                        var oidcService = new OidcService(new HttpClient(), new AzureIdentityService(), handler, govUkOidcConfiguration);

                        var result = oidcService.GetToken(code, redirectUri);
                        var claims = new List<Claim>
                        {
                            new Claim("id_token", result.IdToken),
                            new Claim("access_token", result.AccessToken)
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, notification.Options.SignInAsAuthenticationType);
                        
                        notification.HandleCodeRedemption(result.AccessToken, result.IdToken);

                        var properties =
                            notification.Options.StateDataFormat.Unprotect(notification.ProtocolMessage.State.Split('=')[1]);
                        

                        notification.AuthenticationTicket = new AuthenticationTicket(claimsIdentity, properties);
                        
                        
                        return Task.CompletedTask;
                    },
                    SecurityTokenValidated = notification =>
                    {
                        var oidcService = new OidcService(new HttpClient(), new AzureIdentityService(),
                            handler, govUkOidcConfiguration);
                        
                        oidcService.PopulateAccountClaims(notification.AuthenticationTicket.Identity, notification.ProtocolMessage.AccessToken);

                        

                        return Task.CompletedTask;
                    }
                    
                }
            });

        }
    }
}