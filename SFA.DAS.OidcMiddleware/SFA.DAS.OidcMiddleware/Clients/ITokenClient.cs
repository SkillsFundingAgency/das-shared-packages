using System;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace SFA.DAS.OidcMiddleware.Clients
{
    public interface ITokenClient
    {
        Task<TokenResponse> RequestAuthorizationCodeAsync(string tokenEndpoint, string clientId, string clientSecret,string code, Uri redirectUri);
    }
}