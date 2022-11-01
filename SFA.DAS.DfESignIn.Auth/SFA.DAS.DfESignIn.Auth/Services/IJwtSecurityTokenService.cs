using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace SFA.DAS.DfESignIn.Auth.Services
{
    public interface IJwtSecurityTokenService
    {
        string CreateToken(string clientId, string audience, ClaimsIdentity claimsIdentity,
            SigningCredentials signingCredentials);
    }
}