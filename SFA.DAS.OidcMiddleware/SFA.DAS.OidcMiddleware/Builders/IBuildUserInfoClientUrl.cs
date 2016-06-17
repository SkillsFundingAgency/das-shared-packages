using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.OidcMiddleware.Builders
{
    public interface IBuildUserInfoClientUrl
    {
        Task<IEnumerable<Claim>> GetUserClaims(OidcMiddlewareOptions options, string accessToken);
    }
}