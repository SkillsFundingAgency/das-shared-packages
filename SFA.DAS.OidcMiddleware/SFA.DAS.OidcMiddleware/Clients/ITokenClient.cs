using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace SFA.DAS.OidcMiddleware.Clients
{
    public interface ITokenClient
    {
        Task<TokenResponse> RequestAuthorizationCodeAsync(HttpMessageInvoker httpClient, string tokenEndpoint, string clientId, string clientSecret,string code, Uri redirectUri);
    }
}