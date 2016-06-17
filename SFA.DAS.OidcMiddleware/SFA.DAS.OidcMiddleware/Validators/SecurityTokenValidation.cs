using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
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

        public  List<Claim> ValidateToken(string token, string nonce)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(_options.ClientSecret);
            Array.Resize(ref keyBytes, 64);

            var parameters = new TokenValidationParameters
            {
                ValidAudience = _options.ClientId,
                ValidIssuer = _options.BaseUrl + "/Login",
                IssuerSigningToken = new BinarySecretSecurityToken(keyBytes)
            };

            SecurityToken jwt;
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, parameters, out jwt);

            // validate nonce
            var nonceClaim = principal.FindAll("info").FirstOrDefault(c => c.Value.StartsWith("{\"nonce"))?.Value;

            var nonceValue = JObject.Parse(nonceClaim)["nonce"];


            if (!string.Equals(nonceValue.Value<string>(), nonce, StringComparison.Ordinal))
            {
                throw new Exception("invalid nonce");
            }

            return principal.Claims.ToList();
        }
    }
}