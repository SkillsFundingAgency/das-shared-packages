using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.DfESignIn.Auth.Services;

namespace SFA.DAS.DfESignIn.SampleSite.Standard.AppStart
{
    public class CustomClaims : ICustomClaims
    {
        public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            var value = tokenValidatedContext?.Principal?.Identities.First().Claims
                .FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))
                ?.Value;

            var ukPrn = 10000531;

            List<Claim> claims = List<Claim>();

            claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, ukPrn.ToString()));
            claims.Add(new Claim("http://schemas.portal.com/displayname", "Display Name"));
            claims.Add(new Claim("http://schemas.portal.com/ukprn", ukPrn.ToString()));

            return claims;
        }

        private List<T> List<T>()
        {
            throw new NotImplementedException();
        }
    }
}