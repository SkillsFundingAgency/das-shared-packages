using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Services
{
    public interface ICustomClaims
    {
        Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext);
    }
}