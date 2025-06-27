using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.GovUK.Auth.Validation;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    [ExcludeFromCodeCoverage]
    internal static class ConfigureGovUkAuthenticationExtension
    {
        internal static void ConfigureGovUkAuthentication(this IServiceCollection services, IConfiguration configuration, string redirectUrl, string cookieDomain)
        {
            services.AddScoped(sp =>
            {
                var config = sp.GetRequiredService<IOptions<GovUkOidcConfiguration>>();
                var govUkAuthenticationService = sp.GetRequiredService<IGovUkAuthenticationService>();
                var coreIdentityJwtValidator = sp.GetRequiredService <ICoreIdentityJwtValidator>();

                return new GovUkOpenIdConnectEvents(
                    config,
                    govUkAuthenticationService,
                    coreIdentityJwtValidator,
                    redirectUrl);
            });

            services.AddScoped(sp =>
            {
                var config = sp.GetRequiredService<IOptions<GovUkOidcConfiguration>>();
                var ticketStore = sp.GetRequiredService<ITicketStore>();

                return new GovUkCookieAuthenticationEvents(
                    config,
                    ticketStore);
            });

            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddOpenIdConnect()
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = new PathString("/error/403");
                    options.Cookie.Name = GovUkConstants.AuthCookieName;
                    if (!string.IsNullOrEmpty(cookieDomain))
                    {
                        options.Cookie.Domain = cookieDomain;
                    }
                    options.Cookie.IsEssential = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                    options.LogoutPath = "/home/signed-out";
                });

            services
                .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<GovUkOidcConfiguration, IAzureIdentityService, ValidateCoreIdentityJwtClaimAction>(
                    (options, config, azureIdentityService, validateCoreIdentityJwtClaimAction) =>
                    {
                        var baseUrl = config.BaseUrl;
                        options.ClientId = config.ClientId;
                        options.MetadataAddress = OneLoginUrlHelper.GetMetadataAddress(baseUrl);
                        options.ResponseType = "code";
                        options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                        options.SignedOutRedirectUri = "/";
                        options.SignedOutCallbackPath = "/signed-out";
                        options.CallbackPath = "/sign-in";
                        options.ResponseMode = string.Empty;
                        options.SaveTokens = true;
                        options.GetClaimsFromUserInfoEndpoint = true;
                        options.ClaimActions.Add(validateCoreIdentityJwtClaimAction);

                        var scopes = "openid email phone".Split(' ');
                        options.Scope.Clear();
                        foreach (var scope in scopes)
                        {
                            options.Scope.Add(scope);
                        }

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            AuthenticationType = "private_key_jwt",
                            IssuerSigningKey = new KeyVaultSecurityKey(config.KeyVaultIdentifier,
                                azureIdentityService.AuthenticationCallback),
                            ValidateIssuerSigningKey = true,
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            SaveSigninToken = true,
                            ValidIssuer = OneLoginUrlHelper.GetCoreIdentityClaimIssuer(config.BaseUrl)
                        };

                        options.EventsType = typeof(GovUkOpenIdConnectEvents);
                    });

            services
                .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<GovUkOidcConfiguration, ITicketStore>(
                    (options, config, ticketStore) =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(config.LoginSlidingExpiryTimeOutInMinutes);
                    options.SlidingExpiration = true;
                    options.SessionStore = ticketStore;

                    options.EventsType = typeof(GovUkCookieAuthenticationEvents);
                });
        }
    }
}