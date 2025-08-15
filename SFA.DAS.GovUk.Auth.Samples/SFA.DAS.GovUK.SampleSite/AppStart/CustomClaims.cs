using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.SampleSite.AppStart;

public class CustomClaims : ICustomClaims
{
    public IHttpContextAccessor _httpContextAccessor;

    public CustomClaims(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<Claim?>> GetClaims(TokenValidatedContext tokenValidatedContext)
    {
        return await GetClaims(tokenValidatedContext?.Principal);
    }

    public async Task<IEnumerable<Claim>> GetClaims(ClaimsPrincipal? principal)
    {
        var value = principal?.Identities.First().Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))
            ?.Value;

        var claims = new List<Claim>
        {
            new Claim("CustomClaimId",$"ABC123-{value}"),
            new Claim(ClaimTypes.Name,$"Mr Tester Test")
        };

        // an actual service would read the user details in the service database this sample
        // is using the flag on the index page to indicate suspension
        var suspended = _httpContextAccessor?.HttpContext?.Session.GetString("user:suspended");
        if (string.Equals(suspended, "1", StringComparison.Ordinal))
        {
            claims.Add(new Claim(ClaimTypes.AuthorizationDecision, "Suspended"));
        }

        return await Task.FromResult(claims);
    }
}