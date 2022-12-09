using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Collections.Generic;
using System.Security.Claims;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface ICustomClaims
    {
        IEnumerable<Claim> GetClaims(TokenValidatedContext tokenValidatedContext);
    }
}