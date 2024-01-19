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
            throw new BearTokenException("Signing key cannot be null or empty");
        }

        const int minimumKeySize = 128;
        if (signingKey.Length * 8 < minimumKeySize)
        {
            // This checks the key is at least 128 bits long, otherwise the token will fail to be generated
            throw new BearTokenException("Signing key must exceed 128bits in length");
        }
    }

    private static bool IsUserAuthenticated(ClaimsPrincipal? user)
    {
        if (user == null) return false;

        if (user.Identity == null) return false;

        return user.Identity.IsAuthenticated;
    }
}


public class BearTokenException : Exception
{
    public BearTokenException(string message) : base(message)
    {
    }
}
