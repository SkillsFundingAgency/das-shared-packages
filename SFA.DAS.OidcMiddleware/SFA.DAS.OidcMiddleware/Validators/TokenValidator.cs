using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace SFA.DAS.OidcMiddleware.Validators
{
    public class TokenValidator : ITokenValidator
    {
        public void ValidateToken(OidcMiddlewareOptions options, string token, string nonce)
        {
            var keyBytes = Encoding.UTF8.GetBytes(options.ClientSecret);

            Array.Resize(ref keyBytes, 64);

            var parameters = new TokenValidationParameters
            {
                ValidAudience = options.ClientId,
                ValidIssuer = options.BaseUrl,
            };

            if (options.TokenValidationMethod == TokenValidationMethod.SigningKey)
            {
                if (options.TokenSigningCertificateLoader == null)
                {
                    throw new Exception("Options does not have TokenSigningCertificateLoader, which is required for TokenValidationMethod of SigningKey.");
                }

                parameters.IssuerSigningKeyResolver = (token1, securityToken, keyIdentifier, validationParameters) =>
                {
                    var certificate = options.TokenSigningCertificateLoader();
                    return new[] {new X509SecurityKey(certificate)};
                };
            }
            else
            {
                parameters.IssuerSigningKey = new SymmetricSecurityKey(keyBytes); 
            }

            var principal = new JwtSecurityTokenHandler().ValidateToken(token, parameters, out var jwt);
            var nonceClaim = principal.FindAll("nonce").FirstOrDefault();

            if (!string.Equals(nonceClaim.Value, nonce, StringComparison.Ordinal))
            {
                throw new Exception($"Invalid nonce. :: {nonce} , nonceClaim :: {nonceClaim.Value}");
            }
        }
    }
}