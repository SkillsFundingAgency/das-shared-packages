using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.GovUK.Auth.Services
{
    public class OidcRequestObjectService : IOidcRequestObjectService
    {
        private readonly ISigningCredentialsProvider _signingProvider;

        public OidcRequestObjectService(ISigningCredentialsProvider signingProvider, IConfiguration configuration)
        { 
            _signingProvider = signingProvider;
        }

        public Task<string> BuildRequestJwtAsync(
            string baseUrl,
            string clientId,
            string redirectUri,
            string scopes,
            string state,
            string nonce,
            string[] vtr,
            Dictionary<string, object>? claims = null)
        {
            var now = DateTimeOffset.UtcNow;
            var payload = new JwtPayload
            {
                {"aud", $"{baseUrl}/authorize"},
                {"iss", clientId},
                {"client_id", clientId},
                {"response_type", "code"},
                {"redirect_uri", redirectUri},
                {"scope", scopes},
                {"state", state},
                {"nonce", nonce},
                {"ui_locales", "en"},
                {"vtr", vtr},
                {"exp", now.AddMinutes(5).ToUnixTimeSeconds()},
                {"iat", now.ToUnixTimeSeconds()},
                {"nbf", now.ToUnixTimeSeconds()},
                {"jti", Guid.NewGuid().ToString()}
            };
            
            if (claims != null)
            {
                payload.Add("claims", claims);
            }

            var jwt = new JwtSecurityToken(new JwtHeader(_signingProvider.GetSigningCredentials()), payload);
            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(jwt));
        }
    }
}
