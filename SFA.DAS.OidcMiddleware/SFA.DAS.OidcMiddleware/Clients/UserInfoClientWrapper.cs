using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace SFA.DAS.OidcMiddleware.Clients
{
    public class UserInfoClientWrapper : IUserInfoClient
    {
        public async Task<IEnumerable<Claim>> GetUserClaims(HttpClient client, OidcMiddlewareOptions options, string accessToken)
        {
            var userInfo = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = options.UserInfoEndpoint,
                Token = accessToken
            });
            var claims = userInfo.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();

            return claims;
        }
    }
}