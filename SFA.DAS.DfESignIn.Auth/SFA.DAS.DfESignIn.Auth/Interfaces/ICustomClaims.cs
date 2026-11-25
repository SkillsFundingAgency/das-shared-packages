using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface ICustomClaims
    {
        IEnumerable<Claim> GetClaims(TokenValidatedContext tokenValidatedContext);
        Task<IEnumerable<Claim>> GetClaimsAsync(TokenValidatedContext tokenValidatedContext);
    }
}