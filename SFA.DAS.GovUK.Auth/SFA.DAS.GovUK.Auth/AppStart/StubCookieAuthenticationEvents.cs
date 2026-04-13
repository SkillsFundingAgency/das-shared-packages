using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Extensions;
using SFA.DAS.GovUK.Auth.Helper;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    public class StubCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly GovUkOidcConfiguration _config;
        private readonly ITicketStore _ticketStore;
        private readonly string _signedOutRedirectUrl;
        private readonly string _suspendedRedirectUrl;
        private readonly string _loginRedirect;

        public StubCookieAuthenticationEvents(
            IOptions<GovUkOidcConfiguration> config,
            ITicketStore ticketStore,
            string signedOutRedirectUrl,
            string suspendedRedirectUrl,
            string loginRedirect)
        {
            _config = config.Value;
            _ticketStore = ticketStore;
            _signedOutRedirectUrl = signedOutRedirectUrl;
            _suspendedRedirectUrl = suspendedRedirectUrl;
            _loginRedirect = loginRedirect;
        }

        public override Task SigningIn(CookieSigningInContext context)
        {
            context.Properties.Items["suspended_redirect"] = _suspendedRedirectUrl;
            return Task.CompletedTask;
        }

        public override Task SigningOut(CookieSigningOutContext context)
        {
            context.Response.Cookies.Delete(GovUkConstants.StubAuthCookieName);
            context.Response.Redirect(_signedOutRedirectUrl);
            return Task.CompletedTask;
        }

        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            if (!string.IsNullOrEmpty(_loginRedirect))
            {
                var redirectUri = new Uri(context.RedirectUri);
                var redirectQuery = HttpUtility.UrlEncode(
                    $"{redirectUri.Scheme}://{redirectUri.Authority}{HttpUtility.UrlDecode(redirectUri.Query.Replace("?ReturnUrl=", ""))}");
                context.Response.Redirect(_loginRedirect + "?ReturnUrl=" + redirectQuery);
                return Task.CompletedTask;
            }

            return base.RedirectToLogin(context);
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var identity = (ClaimsIdentity)context.Principal.Identity;
            var ticketUpdated = false;

            var enableVerify = AuthenticationExtension.EnableVerify(_config, context.Properties);
            if (!enableVerify)
            {
                ticketUpdated |= identity.AddOrReplaceClaim("vot", "Cl.Cm", existing => existing.Value == "Cl.Cm");
            }
            else
            {
                ticketUpdated |= identity.AddOrReplaceClaim("vot", "Cl.Cm.P2", existing => existing.Value.Contains("P2"));
                ticketUpdated |= AddStubUserInfoClaims(identity);

                var coreIdentityJwtClaim = identity.FindFirst(UserInfoClaims.CoreIdentityJWT.GetDescription());
                if (coreIdentityJwtClaim != null)
                {
                    var latestName = CoreIdentityJwtClaimHelper.GetLatestNameFromJwtClaim(coreIdentityJwtClaim.Value);
                    if (latestName != null)
                    {
                        if (!string.IsNullOrWhiteSpace(latestName.FullName))
                        {
                            ticketUpdated |= identity.AddOrReplaceClaim(ClaimTypes.Name, latestName.FullName, existing => existing.Value == latestName.FullName);
                        }

                        if (!string.IsNullOrWhiteSpace(latestName.FamilyName))
                        {
                            ticketUpdated |= identity.AddOrReplaceClaim(ClaimTypes.Surname, latestName.FamilyName, existing => existing.Value == latestName.FamilyName);
                        }

                        if (!string.IsNullOrWhiteSpace(latestName.GivenName))
                        {
                            ticketUpdated |= identity.AddOrReplaceClaim(ClaimTypes.GivenName, latestName.GivenName, existing => existing.Value == latestName.GivenName);
                        }
                    }
                }
            }

            if (ticketUpdated && context.Properties.Items.TryGetValue(AuthenticationTicketStore.SessionId, out var sessionId))
            {
                var updatedTicket = new AuthenticationTicket(context.Principal, context.Properties, context.Scheme.Name);
                await _ticketStore.RenewAsync(sessionId, updatedTicket);
                context.ShouldRenew = true;
            }
        }

        private bool AddStubUserInfoClaims(ClaimsIdentity identity)
        {
            var userJson = identity.FindFirst(StubAuthenticationService.StubGovUkUserClaimType)?.Value;
            if (userJson == null)
            {
                return false;
            }

            var ticketUpdated = false;
            var user = JsonSerializer.Deserialize<GovUkUser>(userJson);

            var keys = _config.RequestedUserInfoClaims
                .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var key in keys)
            {
                if (!Enum.TryParse<UserInfoClaims>(key, out var userInfoClaim))
                {
                    continue;
                }

                string value = userInfoClaim switch
                {
                    UserInfoClaims.CoreIdentityJWT => CoreIdentityJwtConverter.SerializeStubCoreIdentityJwt(user.CoreIdentityJwt),
                    UserInfoClaims.Address => JsonSerializer.Serialize(user.Addresses),
                    UserInfoClaims.Passport => JsonSerializer.Serialize(user.Passports),
                    UserInfoClaims.DrivingPermit => JsonSerializer.Serialize(user.DrivingPermits),
                    UserInfoClaims.ReturnCode => JsonSerializer.Serialize(user.ReturnCodes),
                    _ => null
                };

                if (value != null)
                {
                    ticketUpdated |= identity.AddOrReplaceClaim(userInfoClaim.GetDescription(), value, existing => existing.Value == value);
                }
            }

            return ticketUpdated;
        }
    }
}