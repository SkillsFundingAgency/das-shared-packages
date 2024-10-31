using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SFA.DAS.Authorization.TokenGenerator.Providers;


public static class BearerTokenProvider
{
    private static int _expiryTime = 5;

    public static string GetBearerToken(string? signingKey, IEnumerable<Claim> claims)
    {
        ValidateSigningKey(signingKey);


        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: creds,
            expires: DateTime.UtcNow.AddMinutes(_expiryTime)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    private static void ValidateSigningKey(string? signingKey)
    {
        if (string.IsNullOrEmpty(signingKey))
        {
            throw new BearerTokenException("Signing key cannot be null or empty");
        }

        const int minimumKeySize = 128;
        if (signingKey.Length * 8 < minimumKeySize)
        {
            // This checks the key is at least 128 bits long, otherwise the token will fail to be generated
            throw new BearerTokenException("Signing key must be at least 128bits in length");
        }
    }
}


public class BearerTokenException : Exception
{
    public BearerTokenException(string message) : base(message)
    {
    }
}
