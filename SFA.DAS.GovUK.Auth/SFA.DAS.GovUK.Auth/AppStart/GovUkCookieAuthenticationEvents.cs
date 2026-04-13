using System.Security.Claims;
using System.Threading.Tasks;
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
    public class GovUkCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly GovUkOidcConfiguration _config;
        private readonly ITicketStore _ticketStore;
        
        public GovUkCookieAuthenticationEvents(
            IOptions<GovUkOidcConfiguration> config,
            ITicketStore ticketStore)
        {
            _config = config.Value;
            _ticketStore = ticketStore;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            if (!AuthenticationExtension.EnableVerify(_config, context.Properties))
            {
                return;
            }

            var identity = (ClaimsIdentity)context.Principal.Identity;
            var ticketUpdated = false;

            // the docs state that the 'vot' claim only contains the credential trust level 'Cl.Cm', not the level of confidence,
            // which is true, using the presense of the 'coreIdentityJwt' claim which will only be present if the required level of 
            // confidence 'P2' was present and validated, we can imply that the actual vot is 'Cl.Cm.P2', adding 'P2' back so
            // that the 'vot' can be used to determine that the verify authorization policy has succeeded
            var coreIdentityJwtClaim = identity.FindFirst(UserInfoClaims.CoreIdentityJWT.GetDescription());
            if (coreIdentityJwtClaim != null)
            {
                ticketUpdated |= identity.AddOrReplaceClaim("vot", "Cl.Cm.P2", existing => existing.Value.Contains("P2"));

                var latestName = CoreIdentityJwtClaimHelper.GetLatestNameFromJwtClaim(coreIdentityJwtClaim.Value);
                if (latestName != null)
                {
                    if (!string.IsNullOrWhiteSpace(latestName.FullName))
                    {
                        ticketUpdated |= identity.AddOrReplaceClaim(ClaimTypes.Name, latestName.FullName, existing => existing.Value.Equals(latestName.FullName));
                    }

                    if (!string.IsNullOrWhiteSpace(latestName.FamilyName))
                    {
                        ticketUpdated |= identity.AddOrReplaceClaim(ClaimTypes.Surname, latestName.FamilyName, existing => existing.Value.Equals(latestName.FamilyName));
                    }

                    if (!string.IsNullOrWhiteSpace(latestName.GivenName))
                    {
                        ticketUpdated |= identity.AddOrReplaceClaim(ClaimTypes.GivenName, latestName.GivenName, existing => existing.Value.Equals(latestName.GivenName));
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
    }
}