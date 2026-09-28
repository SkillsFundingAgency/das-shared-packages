using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Extensions;
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

            if (identity.HasClaim(GovUkUserClaimTypes.VerifiedIdentity, "true"))
            {
                // the docs state that the 'vot' claim only contains the credential trust level 'Cl.Cm', not the level of confidence,
                // which is true, if the core identity claim was present and validated previously, we can imply that the actual vot
                // is 'Cl.Cm.P2', adding 'P2' back so that the 'vot' can be used to determine that the verify authorization policy
                // has succeeded
                ticketUpdated |= identity.AddOrReplaceClaim(
                    "vot", "Cl.Cm.P2", existing => existing.Value.Contains("P2"));
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