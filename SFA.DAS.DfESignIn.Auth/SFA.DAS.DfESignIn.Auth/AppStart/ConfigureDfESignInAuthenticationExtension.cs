using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
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
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;

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
            string redirectUrl)
        {
            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddOpenIdConnect(options =>
                {
                    var configName = clientName;
                    if (clientName == ClientName.RoatpServiceAdmin)
                    {
                        configName = ClientName.ServiceAdmin;
                    }
                    options.ClientId = configuration[$"DfEOidcConfiguration_{configName}:ClientId"];
                    options.ClientSecret = configuration[$"DfEOidcConfiguration_{configName}:Secret"];
                    options.MetadataAddress = $"{configuration["DfEOidcConfiguration:BaseUrl"]}/.well-known/openid-configuration";
                    options.ResponseType = "code";
                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                    options.SignedOutRedirectUri = redirectUrl;
                    options.SignedOutCallbackPath = new PathString(RoutePath.OidcSignOut);
                    options.CallbackPath = new PathString(RoutePath.OidcSignIn);
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ResponseMode = string.Empty;
                    options.MapInboundClaims = false;
                    options.UseTokenLifetime = true;

                    var scopes = configuration["DfEOidcConfiguration:Scopes"].Split(' ');
                    options.Scope.Clear();
                    foreach (var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        AuthenticationType = OpenIdConnectDefaults.AuthenticationScheme,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = false
                    };

                    options.ProtocolValidator = new OpenIdConnectProtocolValidator
                    {
                        RequireSub = true,
                        RequireStateValidation = false,
                        NonceLifetime = TimeSpan.FromMinutes(60)
                    };

                    options.Events = new OpenIdConnectEvents
                    {
                        OnRemoteFailure = c =>
                        {
                            if (c.Failure != null && c.Failure.Message.Contains("Correlation failed"))
                            {
                                c.Response.Redirect(redirectUrl);
                                c.HandleResponse();
                            }

                            return Task.CompletedTask;
                        },
                        OnSignedOutCallbackRedirect = c =>
                        {
                            c.Response.Cookies.Delete(authenticationCookieName);
                            c.Response.Redirect(c.Options.SignedOutRedirectUri);
                            c.HandleResponse();
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = async ctx => 
                        {
                            var dfeSignInService = ctx.HttpContext.RequestServices.GetRequiredService<IDfESignInService>();
                            await dfeSignInService.PopulateAccountClaims(ctx);
                        }
                    };
                })
                .AddAuthenticationCookie(authenticationCookieName, signedOutCallbackPath, configuration["ResourceEnvironmentName"], clientName);
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