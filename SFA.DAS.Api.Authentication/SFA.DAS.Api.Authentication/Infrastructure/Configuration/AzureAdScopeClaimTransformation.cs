using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace SFA.DAS.Api.Authentication.Infrastructure.Configuration
{
    public class AzureAdScopeClaimTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var scopeClaims = principal.FindAll((string) Constants.ScopeClaimType).ToList();
            if (scopeClaims.Count != 1 || !scopeClaims[0].Value.Contains(' '))
            {
                // Caller has no scopes or has multiple scopes (already split)
                // or they have only one scope
                return Task.FromResult(principal);
            }

            var claim = scopeClaims[0];
            var scopes = claim.Value.Split(' ');
            var claims = scopes.Select(s => new Claim(Constants.ScopeClaimType, s));

            return Task.FromResult(new ClaimsPrincipal(new ClaimsIdentity(principal.Identity, claims)));
        }
    }
}