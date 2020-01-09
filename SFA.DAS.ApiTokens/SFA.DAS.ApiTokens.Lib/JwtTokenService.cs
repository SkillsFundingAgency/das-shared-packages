using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SFA.DAS.ApiTokens.Lib
{
    public class JwtTokenService : IJwtTokenService
    {
        private const string ValueClaimData = "data"; // name of claim to hold the value
        private const string ValueClaimRoles = "roles"; // name of claim to hold the value

        private readonly double _allowedClockSkewInMinutes;
        private readonly InMemorySymmetricSecurityKey _signingKey;

        public JwtTokenService(string secret, double allowedClockSkewInMinutes = 5)
        {
            if (string.IsNullOrWhiteSpace(secret)) throw new ArgumentException("secret cannot be empty");

            _allowedClockSkewInMinutes = allowedClockSkewInMinutes;
            _signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public string Encode(string data, string audience, string issuer, double lifetimeDurationSeconds = 60)
        {
            if (string.IsNullOrWhiteSpace(data)) throw new ArgumentException("data cannot be empty");
            if (string.IsNullOrWhiteSpace(audience)) throw new ArgumentException("audience cannot be empty");
            if (string.IsNullOrWhiteSpace(issuer)) throw new ArgumentException("issuer cannot be empty");
            if (lifetimeDurationSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(lifetimeDurationSeconds), "must be greater than zero");

            var signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ValueClaimData, data));

            // for the roles claims, if there is more than one role in the data parameter, we need a seperate claim per role,
            // the day the data payload used to work was a space separated list which was processed by a custom authorisation handler
            // See ApiKeyHandler. 
            var roles = data.Split(' ');
            foreach (var role in roles)
            {
                claimsIdentity.AddClaim(new Claim(ValueClaimRoles, role));
            }
            

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                AppliesToAddress = audience,
                TokenIssuerName = issuer,
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials,
                Lifetime = new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddSeconds(lifetimeDurationSeconds))
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
            SecurityToken validatedToken;

            var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
            var claim = claimsPrincipal.FindFirst(x => x.Type == ValueClaimData);

            return claim.Value;
        }
    }
}
