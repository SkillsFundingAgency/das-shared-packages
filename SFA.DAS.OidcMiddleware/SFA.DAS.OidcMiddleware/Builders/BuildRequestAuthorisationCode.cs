using System;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace SFA.DAS.OidcMiddleware.Builders
{
    public class BuildRequestAuthorisationCode : IBuildRequestAuthorisationCode
    {
        public async Task<TokenResponse> GetTokenResponse(string tokenEndpoint, string clientId, string clientSecret,string code, Uri redirectUri)
        {
            var client = new TokenClient(tokenEndpoint, clientId, clientSecret);
            
            return await client.RequestAuthorizationCodeAsync(code, $"{redirectUri.Scheme}{Uri.SchemeDelimiter}{redirectUri.Authority}{redirectUri.AbsolutePath}");
        }
    }
}