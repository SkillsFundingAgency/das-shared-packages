using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace SFA.DAS.OidcMiddleware.Clients
{
    public class TokenClientWrapper : ITokenClient
    {
        public async Task<TokenResponse> RequestAuthorizationCodeAsync(HttpMessageInvoker httpClient, string tokenEndpoint, string clientId, string clientSecret, string code, Uri redirectUri)
        {
            var client = new TokenClient(() => httpClient, new TokenClientOptions{Address = tokenEndpoint, ClientId = clientId, ClientSecret = clientSecret});
            
            return await client.RequestAuthorizationCodeTokenAsync(code, $"{redirectUri.Scheme}{Uri.SchemeDelimiter}{redirectUri.Authority}{redirectUri.AbsolutePath}");
        }
    }
}