using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Constants;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using SFA.DAS.DfESignIn.Auth.Enums;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    internal static class ConfigureDfESignInAuthenticationExtension
    {
        internal static void ConfigureDfESignInAuthentication(
            this IServiceCollection services,
            IConfiguration configuration,
            string authenticationCookieName,
            ClientName clientName,
            string signedOutCallbackPath,
            string redirectUrl, 
            string authenticationScheme)
        {
            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddOpenIdConnect(!string.IsNullOrEmpty(authenticationScheme)? authenticationScheme : OpenIdConnectDefaults.AuthenticationScheme,options =>
                {
                    options.ClientId = configuration[$"DfEOidcConfiguration_{clientName}:ClientId"];
                    options.ClientSecret = configuration[$"DfEOidcConfiguration_{clientName}:Secret"];
                    options.MetadataAddress = $"{configuration["DfEOidcConfiguration:BaseUrl"]}/.well-known/openid-configuration";
                    options.ResponseType = "code";
                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                    options.SignedOutRedirectUri = redirectUrl;
                    options.SignedOutCallbackPath = new PathString(RoutePath.OidcSignOut); // the path the authentication provider posts back after signing out.
                    options.CallbackPath = new PathString(RoutePath.OidcSignIn); // the path the authentication provider posts back when authenticating.
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ResponseMode = string.Empty;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        AuthenticationType = OpenIdConnectDefaults.AuthenticationScheme
                    };

                    var scopes = configuration["DfEOidcConfiguration:Scopes"].Split(' ');
                    options.Scope.Clear();
                    foreach (var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }

                    options.SecurityTokenValidator = new JwtSecurityTokenHandler()
                    {
                        InboundClaimTypeMap = new Dictionary<string, string>(),
                        TokenLifetimeInMinutes = 90,
                        SetDefaultTimesOnTokenCreation = true
                    };
                    options.ProtocolValidator = new OpenIdConnectProtocolValidator
                    {
                        RequireSub = true,
                        RequireStateValidation = false,
                        NonceLifetime = TimeSpan.FromMinutes(60)
                    };

                    options.Events.OnRemoteFailure = c =>
                    {
                        if (c.Failure != null && c.Failure.Message.Contains("Correlation failed"))
                        {
                            c.Response.Redirect(redirectUrl);
                            c.HandleResponse();
                        }

                        return Task.CompletedTask;
                    };

                    options.Events.OnSignedOutCallbackRedirect = c =>
                    {
                        c.Response.Cookies.Delete(authenticationCookieName); // delete the client cookie by given cookie name.
                        c.Response.Redirect(c.Options.SignedOutRedirectUri); // the path the authentication provider posts back after signing out.
                        c.HandleResponse();
                        return Task.CompletedTask;
                    };
                })
                .AddAuthenticationCookie(authenticationCookieName, signedOutCallbackPath, configuration["ResourceEnvironmentName"], clientName, authenticationScheme);
            services
                .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<IDfESignInService, IOptions<DfEOidcConfiguration>, ITicketStore>(
                    (options, dfeSignInService, config, ticketStore) =>
                    {
                        options.Events.OnTokenValidated = async ctx => await dfeSignInService.PopulateAccountClaims(ctx);
                    });
            services
                .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<ITicketStore, DfEOidcConfiguration>((options, ticketStore, config) =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(config.LoginSlidingExpiryTimeOutInMinutes);
                    options.SlidingExpiration = true;
                    options.SessionStore = ticketStore;
                });
        }

    }
}