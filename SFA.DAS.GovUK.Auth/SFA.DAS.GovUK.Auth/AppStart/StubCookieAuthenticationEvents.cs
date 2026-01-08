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
            var enableVerify = AuthenticationExtension.EnableVerify(_config, context.Properties);
            var identity = (ClaimsIdentity)context.Principal.Identity;
            var existingVot = identity.FindFirst("vot");

            if (existingVot == null)
            {
                // verify is being enabled on first sign in 
                var vot = enableVerify ? "Cl.Cm.P2" : "Cl.Cm";
                identity.AddClaim(new Claim("vot", vot));
                AddStubUserInfoClaims(enableVerify, identity);
                context.ShouldRenew = true;
            }
            else if (enableVerify && !existingVot.Value.Contains("P2"))
            {
                // verify is being enabled on subsequent sign in
                identity.RemoveClaim(existingVot);
                identity.AddClaim(new Claim("vot", "Cl.Cm.P2"));
                AddStubUserInfoClaims(enableVerify, identity);
                context.ShouldRenew = true;
            }

            if (context.ShouldRenew && context.Properties.Items.TryGetValue(AuthenticationTicketStore.SessionId, out var sessionId))
            {
                var updatedTicket = new AuthenticationTicket(context.Principal, context.Properties, context.Scheme.Name);
                await _ticketStore.RenewAsync(sessionId, updatedTicket);
            }
        }

        private void AddStubUserInfoClaims(bool enableVerify, ClaimsIdentity identity)
        {
            if (!enableVerify) return;

            var userJson = identity.FindFirst(StubAuthenticationService.StubGovUkUserClaimType)?.Value;
            if (userJson == null) return;

            var user = JsonSerializer.Deserialize<GovUkUser>(userJson);

            var keys = _config.RequestedUserInfoClaims
                .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var key in keys)
            {
                if (!Enum.TryParse<UserInfoClaims>(key, out var userInfoClaim)) continue;

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
                    identity.AddClaim(new Claim(userInfoClaim.GetDescription(), value));
            }
        }
    }
}