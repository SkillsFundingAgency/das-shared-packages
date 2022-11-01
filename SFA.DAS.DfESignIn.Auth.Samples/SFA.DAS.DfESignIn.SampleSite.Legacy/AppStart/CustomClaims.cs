using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.DfESignIn.Auth.Services;

namespace SFA.DAS.DfESignIn.SampleSite.AppStart
{
    public class CustomClaims : ICustomClaims
    {
        public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            var value = tokenValidatedContext?.Principal?.Identities.First().Claims
                .FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))
                ?.Value;
            return new List<Claim> {new Claim("EmployerAccountId", $"ABC123-{value}")};
        }
    }
}