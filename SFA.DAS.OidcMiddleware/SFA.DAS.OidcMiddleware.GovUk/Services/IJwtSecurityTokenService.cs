using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SFA.DAS.OidcMiddleware.GovUk.Services
{
    public interface IJwtSecurityTokenService
    {
        string CreateToken(ClaimsIdentity claimsIdentity,
            SigningCredentials signingCredentials);
    }
}