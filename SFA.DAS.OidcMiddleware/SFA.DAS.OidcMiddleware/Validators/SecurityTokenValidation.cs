using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security.Tokens;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SFA.DAS.OidcMiddleware.Validators
{
    public class SecurityTokenValidation : ISecurityTokenValidation
    {
        private readonly OidcMiddlewareOptions _options;


        public SecurityTokenValidation(OidcMiddlewareOptions options)
        {
            _options = options;
        }

        public List<Claim> ValidateToken(string token, string nonce)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(_options.ClientSecret);
            Array.Resize(ref keyBytes, 64);

            var parameters = new TokenValidationParameters
            {
                ValidAudience = _options.ClientId,
                ValidIssuer = _options.BaseUrl,
            };

            if (_options.TokenValidationMethod == TokenValidationMethod.SigningKey)
            {
                if (_options.TokenSigningCertificateLoader == null)
                {
                    throw new Exception(
                        "Options does not have TokenSigningCertificateLoader, which is required for TokenValidationMethod of SigningKey");
                }
                parameters.IssuerSigningKeyResolver = (token1, securityToken, keyIdentifier, validationParameters) =>
                {
                    var certificate = _options.TokenSigningCertificateLoader();
                    return new X509SecurityKey(certificate);
                };
            }
            else
            {
                parameters.IssuerSigningToken = new BinarySecretSecurityToken(keyBytes);
            }

            SecurityToken jwt;
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, parameters, out jwt);
            var nonceClaim = principal.FindAll("nonce").FirstOrDefault();

            if (!string.Equals(nonceClaim.Value, nonce, StringComparison.Ordinal))
            {
                throw new Exception("invalid nonce");
            }

            return principal.Claims.ToList();
        }
    }
}