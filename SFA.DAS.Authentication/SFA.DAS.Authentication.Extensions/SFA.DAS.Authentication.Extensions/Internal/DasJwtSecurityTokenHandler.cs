using SFA.DAS.NLog.Logger;
using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;

namespace SFA.DAS.Authentication.Extensions
{
    internal class DasJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        private readonly ILog _logger;

        public DasJwtSecurityTokenHandler(ILog logger)
        {
            _logger = logger;
        }

        public override ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            try
            {
                var result = base.ValidateToken(securityToken, validationParameters, out validatedToken);

                if (result.Identity is ClaimsIdentity)
                {
                    var identity = (ClaimsIdentity)result.Identity;

                    var dataClaim = identity.FindFirst("data")?.Value;
                    if (dataClaim != null)
                    {
                        var roles = dataClaim.Split(' ');
                        identity.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to validate the security token");
                throw;
            }
        }
    }
}
