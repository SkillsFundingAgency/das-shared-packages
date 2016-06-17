using System.Collections.Generic;
using System.Security.Claims;

namespace SFA.DAS.OidcMiddleware.Validators
{
    public interface ISecurityTokenValidation
    {
        List<Claim> ValidateToken(string token, string nonce);
    }
}