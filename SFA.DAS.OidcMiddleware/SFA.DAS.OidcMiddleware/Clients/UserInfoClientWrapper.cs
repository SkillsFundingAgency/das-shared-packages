using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace SFA.DAS.OidcMiddleware.Clients
{
    public class UserInfoClientWrapper : IUserInfoClient
    {
        public async Task<IEnumerable<Claim>> GetUserClaims(OidcMiddlewareOptions options, string accessToken)
        {
            var userInfoClient = new UserInfoClient(new Uri(options.UserInfoEndpoint), accessToken);
            var userInfo = await userInfoClient.GetAsync();
            var claims = userInfo.Claims.Select(c => new Claim(c.Item1, c.Item2)).ToList();

            return claims;
        }
    }
}