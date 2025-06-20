using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.SampleSite.AppStart;

public class CustomClaims : ICustomClaims
{
    public async Task<IEnumerable<Claim?>> GetClaims(TokenValidatedContext tokenValidatedContext)
    {
        return await GetClaims(tokenValidatedContext?.Principal);
    }

    public async Task<IEnumerable<Claim>> GetClaims(ClaimsPrincipal principal)
    {
        var value = principal?.Identities.First().Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))
            ?.Value;
    
        return new List<Claim>
        {
            new Claim("CustomClaimId",$"ABC123-{value}"),
            new Claim(ClaimTypes.Name,$"Mr Tester Test")
        };
    }
}