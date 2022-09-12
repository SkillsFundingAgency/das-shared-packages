using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SFA.DAS.OidcMiddleware.GovUk.Services
{
    public interface IJwtSecurityTokenService
    {
        string CreateToken(string clientId, string audience, ClaimsIdentity claimsIdentity,
            SigningCredentials signingCredentials);
    }
}