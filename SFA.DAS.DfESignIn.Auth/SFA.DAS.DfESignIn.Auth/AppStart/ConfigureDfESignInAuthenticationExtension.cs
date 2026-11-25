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
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    internal static class ConfigureDfESignInAuthenticationExtension
    {
        /// <summary>
        /// Configures DfE Sign-in using the legacy ClientName enum.
        /// </summary>
        /// <remarks>
        /// This overload uses the package-owned ClientName enum to select credentials
        /// and calculate the cookie domain. Use the overload accepting
        /// DfEOidcConfiguration to supply the combined shared and client settings,
        /// including an explicitly configured cookie domain and sign-out destination.
        /// </remarks>
        [Obsolete(
            "Use the overload accepting DfEOidcConfiguration. " +
            "Client credentials, cookie domains and sign-out destinations should be " +
            "supplied through configuration rather than package-owned client mappings.")]
        internal static void ConfigureDfESignInAuthentication(
            this IServiceCollection services,
            IConfiguration configuration,
            string authenticationCookieName,
            ClientName clientName,
            string signedOutCallbackPath,
            string redirectUrl)
        {
            var configName = clientName;

            if (clientName == ClientName.RoatpServiceAdmin)
            {
                configName = ClientName.ServiceAdmin;
            }

            var oidcConfiguration = new DfEOidcConfiguration
            {
                BaseUrl = configuration["DfEOidcConfiguration:BaseUrl"],
                ClientId = configuration[$"DfEOidcConfiguration_{configName}:ClientId"],
                Secret = configuration[$"DfEOidcConfiguration_{configName}:Secret"],
                Scopes = configuration["DfEOidcConfiguration:Scopes"],
                CookieDomain = RedirectExtension.GetEnvironmentAndDomain(
                    configuration["ResourceEnvironmentName"],
                    clientName),
                SignedOutRedirectUrl = redirectUrl
            };

            services.ConfigureDfESignInAuthentication(
                oidcConfiguration,
                authenticationCookieName,
                signedOutCallbackPath);
        }

        internal static void ConfigureDfESignInAuthentication(
            this IServiceCollection services,
            DfEOidcConfiguration configuration,
            string authenticationCookieName,
            string signedOutCallbackPath)
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
                    options.ClientId = configuration.ClientId;
                    options.ClientSecret = configuration.Secret;
                    options.MetadataAddress = $"{configuration.BaseUrl}/.well-known/openid-configuration";
                    options.ResponseType = "code";
                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                    options.SignedOutRedirectUri = configuration.SignedOutRedirectUrl;
                    options.SignedOutCallbackPath = new PathString(RoutePath.OidcSignOut);
                    options.CallbackPath = new PathString(RoutePath.OidcSignIn);
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ResponseMode = string.Empty;
                    options.MapInboundClaims = false;
                    options.UseTokenLifetime = true;

                    var scopes = configuration.Scopes.Split(' ');

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
                            if (c.Failure != null &&
                                c.Failure.Message.Contains("Correlation failed"))
                            {
                                c.Response.Redirect(configuration.SignedOutRedirectUrl);
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
                            var dfeSignInService = ctx.HttpContext.RequestServices
                                .GetRequiredService<IDfESignInService>();

                            await dfeSignInService.PopulateAccountClaims(ctx);
                        },
                        OnTicketReceived = context => context.PopulateCustomClaimsAsync(),
                    };
                })
                .AddAuthenticationCookie(
                    authenticationCookieName,
                    signedOutCallbackPath,
                    configuration.CookieDomain);

            services
                .AddOptions<OpenIdConnectOptions>(
                    OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<IDfESignInService, IOptions<DfEOidcConfiguration>, ITicketStore>(
                    (options, dfeSignInService, config, ticketStore) =>
                    {
                        options.Events.OnTokenValidated =
                            async ctx => await dfeSignInService.PopulateAccountClaims(ctx);
                    });

            services
                .AddOptions<CookieAuthenticationOptions>(
                    CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<ITicketStore, DfEOidcConfiguration>(
                    (options, ticketStore, config) =>
                    {
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(
                            config.LoginSlidingExpiryTimeOutInMinutes);

                        options.SlidingExpiration = true;
                        options.SessionStore = ticketStore;
                    });
        }
    }
}