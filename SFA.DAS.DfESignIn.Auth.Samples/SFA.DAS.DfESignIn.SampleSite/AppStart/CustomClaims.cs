using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.DfESignIn.SampleSite.AppStart
{
    public class CustomClaims : ICustomClaims
    {
        public IEnumerable<Claim> GetClaims(
            TokenValidatedContext tokenValidatedContext)
        {
            return new List<Claim>
            {
                new("sample:custom-claims", "added")
            };
        }

        public Task<IEnumerable<Claim>> GetClaimsAsync(
            TokenValidatedContext tokenValidatedContext)
        {
            return Task.FromResult(GetClaims(tokenValidatedContext));
        }
    }
}