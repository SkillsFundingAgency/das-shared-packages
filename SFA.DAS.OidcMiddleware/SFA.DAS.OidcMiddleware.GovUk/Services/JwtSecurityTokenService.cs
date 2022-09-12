using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SFA.DAS.OidcMiddleware.GovUk.Services
{
    public class JwtSecurityTokenService : IJwtSecurityTokenService
    {
        public string CreateToken(string clientId, string audience, ClaimsIdentity claimsIdentity,
            SigningCredentials signingCredentials)
        {
            var handler = new JwtSecurityTokenHandler();
            var value = handler.CreateJwtSecurityToken(clientId, audience, claimsIdentity, DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(5), DateTime.UtcNow, signingCredentials);

            return value.RawData;
        }
    }
}