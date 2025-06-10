using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Extensions;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.GovUK.Auth.Validation;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    internal static class ConfigureGovUkAuthenticationExtension
    {
        internal static void ConfigureGovUkAuthentication(this IServiceCollection services, IConfiguration configuration, string redirectUrl, string notVerifiedUrl, string cookieDomain)
        {
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
                .Configure<GovUkOidcConfiguration, ICoreIdentityHelper, ValidateCoreIdentityJwtClaimAction>(
                    (options, govUkOidcConfiguration, coreIdentityHelper, validateCoreIdentityJwtClaimAction) =>
                    {
                        var baseUrl = govUkOidcConfiguration.BaseUrl;
                        options.ClientId = govUkOidcConfiguration.ClientId;
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

                        options.Events.OnRemoteFailure = c =>
                        {
                            if (EnableVerify(govUkOidcConfiguration, c.Properties))
                            {
                                c.Response.Redirect(notVerifiedUrl);
                                c.HandleResponse();
                            }
                            else
                            {
                                if (c.Failure != null && c.Failure.Message.Contains("Correlation failed"))
                                {
                                    c.Response.Redirect("/");
                                    c.HandleResponse();
                                }
                            }

                            return Task.CompletedTask;
                        };

                        options.Events.OnRedirectToIdentityProvider = async c =>
                        {
                            var disable2Fa = Disable2Fa(govUkOidcConfiguration);
                            var enableVerify = EnableVerify(govUkOidcConfiguration, c.Properties);

                            var vtr = disable2Fa ? "Cl" : "Cl.Cm";
                            var stringVtr = JsonSerializer.Serialize(new List<string>
                            {
                                enableVerify ? vtr + ".P2" : vtr
                            });

                            if (c.ProtocolMessage.Parameters.ContainsKey("vtr"))
                            {
                                c.ProtocolMessage.Parameters.Remove("vtr");
                            }

                            c.ProtocolMessage.Parameters.Add("vtr", stringVtr);

                            if (enableVerify)
                            {
                                var userInfoClaimKeys = govUkOidcConfiguration.RequestedUserInfoClaims
                                    .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                                var claims = new Dictionary<string, Dictionary<string, object>>
                                {
                                    ["userinfo"] = new Dictionary<string, object>()
                                };

                                foreach (var key in userInfoClaimKeys)
                                {
                                    if (Enum.TryParse<UserInfoClaims>(key, out var enumValue))
                                    {
                                        var description = enumValue.GetDescription();
                                        if (!string.IsNullOrEmpty(description))
                                        {
                                            claims["userinfo"][description] = null;
                                        }
                                    }
                                }

                                c.ProtocolMessage.Parameters.Add("claims", JsonSerializer.Serialize(claims));
                            }

                            if (c.ProtocolMessage.State == null)
                            {
                                c.ProtocolMessage.State = c.Options.StateDataFormat.Protect(c.Properties);
                            }
                        };

                        options.Events.OnTokenResponseReceived = async ctx =>
                        {
                            if (ctx.TokenEndpointResponse.IdToken is string idToken)
                            {
                                ctx.Properties?.StoreTokens(
                                [
                                    new AuthenticationToken
                                    {
                                        Name = OpenIdConnectParameterNames.IdToken,
                                        Value = idToken
                                    }
                                ]);
                            }

                            if(EnableVerify(govUkOidcConfiguration, ctx.Properties))
                            {
                                await coreIdentityHelper.EnsureDidDocument();
                            }
                        };
                    });

            services
                .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<GovUkOidcConfiguration, IOidcService, IAzureIdentityService, ICustomClaims, ITicketStore>(
                    (options, govUkOidcConfiguration, oidcService, azureIdentityService, customClaims, ticketStore) =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            AuthenticationType = "private_key_jwt",
                            IssuerSigningKey = new KeyVaultSecurityKey(govUkOidcConfiguration.KeyVaultIdentifier,
                                azureIdentityService.AuthenticationCallback),
                            ValidateIssuerSigningKey = true,
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            SaveSigninToken = true,
                            ValidIssuer = OneLoginUrlHelper.GetCoreIdentityClaimIssuer(govUkOidcConfiguration.BaseUrl)
                        };

                        options.Events.OnAuthorizationCodeReceived = async (ctx) =>
                        {
                            var token = await oidcService.GetToken(ctx.TokenEndpointRequest);
                            if (token?.AccessToken != null && token.IdToken != null)
                            {   
                                ctx.Properties.StoreTokens(new[]
                                {
                                    new AuthenticationToken { Name = "access_token", Value = token.AccessToken },
                                    new AuthenticationToken { Name = "id_token", Value = token.IdToken },
                                });

                                ctx.HandleCodeRedemption(token.AccessToken, token.IdToken);
                            }
                        };

                        options.Events.OnSignedOutCallbackRedirect = c =>
                        {
                            c.Response.Cookies.Delete(GovUkConstants.AuthCookieName);
                            c.Response.Redirect(redirectUrl);
                            c.HandleResponse();
                            return Task.CompletedTask;
                        };

                        options.Events.OnTokenValidated = async ctx =>
                        {
                            await oidcService.PopulateAccountClaims(ctx);
                        };
                    });

            services
                .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<GovUkOidcConfiguration, ITicketStore>(
                    (options, govUkOidcConfiguration, ticketStore) =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(govUkOidcConfiguration.LoginSlidingExpiryTimeOutInMinutes);
                    options.SlidingExpiration = true;
                    options.SessionStore = ticketStore;

                    var baseEvents = options.Events ?? new CookieAuthenticationEvents();

                    baseEvents.OnValidatePrincipal = async ctx =>
                    {
                        if (EnableVerify(govUkOidcConfiguration, ctx.Properties))
                        {
                            var identity = (ClaimsIdentity)ctx.Principal.Identity;
                            if (identity.HasClaim(c => c.Type == UserInfoClaims.CoreIdentityJWT.GetDescription()))
                            {
                                var existingVot = identity.FindFirst("vot");
                                if (existingVot != null && !existingVot.Value.Contains("P2"))
                                {
                                    identity.RemoveClaim(existingVot);
                                    identity.AddClaim(new Claim("vot", "Cl.Cm.P2"));

                                    if (ctx.Properties.Items.TryGetValue(AuthenticationTicketStore.SessionId, out var sessionId))
                                    {
                                        var updatedTicket = new AuthenticationTicket(ctx.Principal, ctx.Properties, ctx.Scheme.Name);
                                        await ticketStore.RenewAsync(sessionId, updatedTicket);
                                        ctx.ShouldRenew = true;
                                    }
                                }
                            }
                        }
                    };

                    options.Events = baseEvents;
                });
        }

        internal static bool EnableVerify(GovUkOidcConfiguration govUkOidcConfiguration, AuthenticationProperties authenticationProperties)
        {
            var enabledByConfig = govUkOidcConfiguration.EnableVerify != null && 
                govUkOidcConfiguration.EnableVerify.Equals("true", StringComparison.CurrentCultureIgnoreCase);

            var enabledByProperties = authenticationProperties.Items.TryGetValue("enableVerify", out var flag)
                    && string.Equals(flag, true.ToString(), StringComparison.OrdinalIgnoreCase);

            return enabledByConfig || enabledByProperties;
        }

        internal static bool Disable2Fa(GovUkOidcConfiguration govUkOidcConfiguration)
        {
            var disabled2FaByConfig = govUkOidcConfiguration.Disable2Fa != null && 
                govUkOidcConfiguration.Disable2Fa.Equals("true", StringComparison.CurrentCultureIgnoreCase);
            
            return disabled2FaByConfig;
        }
    }
}