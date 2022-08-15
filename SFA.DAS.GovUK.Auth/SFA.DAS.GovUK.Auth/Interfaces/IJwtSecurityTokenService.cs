using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SFA.DAS.GovUK.Auth.Interfaces;

public interface IJwtSecurityTokenService
{
    string CreateToken(string clientId, string audience, ClaimsIdentity claimsIdentity, SigningCredentials signingCredentials);
}