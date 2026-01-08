using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.GovUK.Auth.Helper;

namespace SFA.DAS.GovUK.Auth.Services
{
    internal class JwtSecurityTokenService : IJwtSecurityTokenService
    {
        private readonly IDateTimeHelper _dateTimeHelper;

        public JwtSecurityTokenService(IDateTimeHelper dateTimeHelper)
        {
            _dateTimeHelper = dateTimeHelper;
        }

        public string CreateToken(string clientId, string audience, ClaimsIdentity claimsIdentity,
            SigningCredentials signingCredentials)
        {
            var handler = new JwtSecurityTokenHandler();
            var value = handler.CreateJwtSecurityToken(clientId, audience, claimsIdentity, _dateTimeHelper.UtcNow,
                _dateTimeHelper.UtcNow.AddMinutes(5), _dateTimeHelper.UtcNow, signingCredentials);

            return value.RawData;
        }
    }
}