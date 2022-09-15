using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.OidcMiddleware.GovUk.Models;

namespace SFA.DAS.OidcMiddleware.GovUk.Services
{
    public interface IOidcService
    {
        Token GetToken(string code, string redirectUri);
        void PopulateAccountClaims(ClaimsIdentity claimsIdentity, string accessToken);
    }
}