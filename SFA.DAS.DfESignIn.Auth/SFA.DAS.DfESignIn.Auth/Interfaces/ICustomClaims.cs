using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface ICustomClaims
    {
        IEnumerable<Claim> GetClaims(TokenValidatedContext tokenValidatedContext);
    }
}