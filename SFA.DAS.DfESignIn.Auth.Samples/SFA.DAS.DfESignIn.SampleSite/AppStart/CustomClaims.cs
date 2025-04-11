using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.DfESignIn.SampleSite.AppStart;

public class CustomClaims : ICustomClaims
{
    public IEnumerable<Claim?> GetClaims(TokenValidatedContext tokenValidatedContext)
    {
        var ukPrn = 10000531;

        return new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, ukPrn.ToString()),
            new("http://schemas.portal.com/displayname", "Display Name"),
            new("http://schemas.portal.com/ukprn", ukPrn.ToString())
        };
    }
}