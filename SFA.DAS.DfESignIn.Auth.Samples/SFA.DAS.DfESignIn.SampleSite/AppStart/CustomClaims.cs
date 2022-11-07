using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.DfESignIn.SampleSite.AppStart;

public class CustomClaims : ICustomClaims
{
    public async Task<IEnumerable<Claim?>> GetClaims(TokenValidatedContext tokenValidatedContext)
    {
        var ukPrn = 10000531;

        List<Claim> claims = new()
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, ukPrn.ToString()),
            new Claim("http://schemas.portal.com/displayname", "Display Name"),
            new Claim("http://schemas.portal.com/ukprn", ukPrn.ToString())
        };

        return claims;
    }
}