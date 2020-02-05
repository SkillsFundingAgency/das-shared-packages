using System.Threading.Tasks;
using System.Text;
using SFA.DAS.Http.Configuration;
using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace SFA.DAS.Http.TokenGenerators
{
    public class GenericJwtBearerTokenGenerator : IGenerateBearerToken
    {
        private readonly IGenericJwtClientConfiguration _config;

        public GenericJwtBearerTokenGenerator(IGenericJwtClientConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<string> Generate()
        {
            var handler = new JwtSecurityTokenHandler();
            var signingSecret = Encoding.UTF8.GetBytes(_config.ClientSecret);
            var securitytokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config.Issuer,
                Audience = _config.Audience,
                Expires = DateTime.UtcNow.AddSeconds(_config.TokenExpirySeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingSecret), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(securitytokenDescriptor);

            return await Task.Run(() =>
            {
                return handler.WriteToken(token);
            });
        }
    }
}
