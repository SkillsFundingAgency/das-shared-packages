using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Extensions;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    internal static class ConfigureGovUkStubAuthenticationExtension
    {

        public static void ConfigureStubAuthentication(this IServiceCollection services, string signedOutRedirectUrl, string loginRedirect, string localStubLoginPath, string cookieDomain)
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = localStubLoginPath;
                    options.AccessDeniedPath = new PathString("/error/403");
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = GovUkConstants.StubAuthCookieName;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    if (!string.IsNullOrEmpty(cookieDomain))
                    {
                        options.Cookie.Domain = cookieDomain;
                    }
                    options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                    options.LogoutPath = "/home/signed-out";
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

                        baseEvents.OnSigningOut = c =>
                        {
                            c.Response.Cookies.Delete(GovUkConstants.StubAuthCookieName);
                            c.Response.Redirect(signedOutRedirectUrl);
                            return Task.CompletedTask;
                        };

                        if (!string.IsNullOrEmpty(loginRedirect))
                        {
                            baseEvents.OnRedirectToLogin = c =>
                            {
                                var redirectUri = new Uri(c.RedirectUri);

                                var redirectQuery = HttpUtility.UrlEncode(
                                    $"{redirectUri.Scheme}://{redirectUri.Authority}{HttpUtility.UrlDecode(redirectUri.Query.Replace("?ReturnUrl=", ""))}");
                                c.Response.Redirect(loginRedirect + "?ReturnUrl=" + redirectQuery);
                                return Task.CompletedTask;
                            };
                        }

                        baseEvents.OnValidatePrincipal = async ctx =>
                        {
                            var enableVerify = EnableVerify(govUkOidcConfiguration, ctx.Properties);
                            
                            var identity = (ClaimsIdentity)ctx.Principal.Identity;
                            var existingVot = identity.FindFirst("vot");

                            if (existingVot == null)
                            {
                                var vot = enableVerify ? "Cl.Cm.P2" : "Cl.Cm";
                                identity.AddClaim(new Claim("vot", vot));
                                AddStubUserInfoClaims(enableVerify, identity, govUkOidcConfiguration);
                                ctx.ShouldRenew = true;
                            }
                            else if (enableVerify && !existingVot.Value.Contains("P2"))
                            {
                                identity.RemoveClaim(existingVot);
                                identity.AddClaim(new Claim("vot", "Cl.Cm.P2"));
                                AddStubUserInfoClaims(enableVerify, identity, govUkOidcConfiguration);
                                ctx.ShouldRenew = true;
                            }

                            if (ctx.ShouldRenew && ctx.Properties.Items.TryGetValue(AuthenticationTicketStore.SessionId, out var sessionId))
                            {
                                var updatedTicket = new AuthenticationTicket(ctx.Principal, ctx.Properties, ctx.Scheme.Name);
                                await ticketStore.RenewAsync(sessionId, updatedTicket);
                            }
                        };

                        options.Events = baseEvents;
                    });
        }

        private static void AddStubUserInfoClaims(bool enableVerify, ClaimsIdentity claimsIdentity, GovUkOidcConfiguration govUkOidcConfiguration)
        {
            if (!enableVerify)
                return;

            var govUkUserClaim = claimsIdentity.FindFirst(StubAuthenticationService.StubGovUkUserClaimType)?.Value;
            if (govUkUserClaim == null)
                return;

            var govUkUser = JsonSerializer.Deserialize<GovUkUser>(govUkUserClaim);
            
            var userInfoClaimKeys = govUkOidcConfiguration.RequestedUserInfoClaims
            .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var key in userInfoClaimKeys)
            {
                if (Enum.TryParse<UserInfoClaims>(key, out var enumValue))
                {
                    if (enumValue == UserInfoClaims.CoreIdentityJWT)
                    {
                        claimsIdentity
                            .AddClaim(
                                new Claim(enumValue.GetDescription(),
                                CoreIdentityJwtConverter.SerializeStubCoreIdentityJwt(govUkUser.CoreIdentityJwt)));
                    }

                    if (enumValue == UserInfoClaims.Address)
                    {
                        claimsIdentity
                            .AddClaim(
                                new Claim(enumValue.GetDescription(),
                                JsonSerializer.Serialize(govUkUser.Addresses)));
                    }

                    if (enumValue == UserInfoClaims.Passport)
                    {
                        claimsIdentity
                            .AddClaim(
                                new Claim(enumValue.GetDescription(),
                                JsonSerializer.Serialize(govUkUser.Passports)));
                    }

                    if (enumValue == UserInfoClaims.DrivingPermit)
                    {
                        claimsIdentity
                            .AddClaim(
                                new Claim(enumValue.GetDescription(),
                                JsonSerializer.Serialize(govUkUser.DrivingPermits)));
                    }

                    if (enumValue == UserInfoClaims.ReturnCode)
                    {
                        claimsIdentity
                            .AddClaim(
                                new Claim(enumValue.GetDescription(),
                                JsonSerializer.Serialize(govUkUser.ReturnCodes)));
                    }
                }
            }
        }

        internal static bool EnableVerify(GovUkOidcConfiguration govUkOidcConfiguration, AuthenticationProperties authenticationProperties)
        {
            var enabledByConfig = govUkOidcConfiguration.EnableVerify != null &&
                govUkOidcConfiguration.EnableVerify.Equals("true", StringComparison.CurrentCultureIgnoreCase);

            var enabledByProperties = authenticationProperties.Items.TryGetValue("enableVerify", out var flag)
                    && string.Equals(flag, true.ToString(), StringComparison.OrdinalIgnoreCase);

            return enabledByConfig || enabledByProperties;
        }
    }
}