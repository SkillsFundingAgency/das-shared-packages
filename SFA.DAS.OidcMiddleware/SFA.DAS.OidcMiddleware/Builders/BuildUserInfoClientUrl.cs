using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace SFA.DAS.OidcMiddleware.Builders
{
    public class BuildUserInfoClientUrl : IBuildUserInfoClientUrl
    {
        public async Task<IEnumerable<Claim>> GetUserClaims(OidcMiddlewareOptions options, string accessToken)
        {
            var userInfoClient = new UserInfoClient(new Uri(options.UserInfoEndpoint), accessToken);

            var userInfo = await userInfoClient.GetAsync();

            var claims = new List<Claim>();
            userInfo.Claims.ToList().ForEach(ui => claims.Add(new Claim(ui.Item1, ui.Item2)));

            return claims;
        }
    }
}