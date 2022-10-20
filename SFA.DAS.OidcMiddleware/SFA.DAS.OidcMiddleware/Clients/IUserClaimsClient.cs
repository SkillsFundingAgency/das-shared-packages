using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.OidcMiddleware.Clients
{
    public interface IUserInfoClient
    {
        Task<IEnumerable<Claim>> GetUserClaims(HttpClient httpClient, OidcMiddlewareOptions options, string accessToken);
    }
}