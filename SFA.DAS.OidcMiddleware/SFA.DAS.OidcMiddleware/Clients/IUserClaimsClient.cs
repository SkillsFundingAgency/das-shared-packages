using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.OidcMiddleware.Clients
{
    public interface IUserInfoClient
    {
        Task<IEnumerable<Claim>> GetUserClaims(OidcMiddlewareOptions options, string accessToken);
    }
}