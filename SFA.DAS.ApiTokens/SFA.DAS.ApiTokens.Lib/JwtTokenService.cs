using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SFA.DAS.ApiTokens.Lib;

public class JwtTokenService : IJwtTokenService
{
    private const string ValueClaimData = "data"; // name of claim to hold the value
    private const string ValueClaimRoles = "roles"; // name of claim to hold the value

    private readonly double _allowedClockSkewInMinutes;
    private readonly SymmetricSecurityKey _signingKey;

    public JwtTokenService(string secret, double allowedClockSkewInMinutes = 5)
    {
        if (string.IsNullOrWhiteSpace(secret)) throw new ArgumentException("secret cannot be empty");

        _allowedClockSkewInMinutes = allowedClockSkewInMinutes;
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }

    public string Encode(string data, string audience, string issuer, double lifetimeDurationSeconds = 60)
    {
        if (string.IsNullOrWhiteSpace(data)) throw new ArgumentException("data cannot be empty");
        if (string.IsNullOrWhiteSpace(audience)) throw new ArgumentException("audience cannot be empty");
        if (string.IsNullOrWhiteSpace(issuer)) throw new ArgumentException("issuer cannot be empty");
        if (lifetimeDurationSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(lifetimeDurationSeconds), "must be greater than zero");

        var signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ValueClaimData, data)
        };

        // for the roles claims, if there is more than one role in the data parameter, we need a separate claim per role,
        // the data payload used to work as a space separated list which was processed by a custom authorisation handler
        // See ApiKeyHandler. 
        var roles = data.Split(' ');
        foreach (var role in roles)
        {
            claims.Add(new Claim(ValueClaimRoles, role));
        }

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = audience,
            Issuer = issuer,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = signingCredentials,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(lifetimeDurationSeconds)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
        var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);

        return signedAndEncodedToken;
    }

    public string Decode(string token, IEnumerable<string> validAudiences, IEnumerable<string> validIssuers)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidAudiences = validAudiences,
            ValidIssuers = validIssuers,
            IssuerSigningKey = _signingKey,
            ClockSkew = TimeSpan.FromMinutes(_allowedClockSkewInMinutes)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
        var claim = claimsPrincipal.FindFirst(x => x.Type == ValueClaimData);

        return claim?.Value ?? throw new SecurityTokenException("Data claim not found in token");
    }
}
