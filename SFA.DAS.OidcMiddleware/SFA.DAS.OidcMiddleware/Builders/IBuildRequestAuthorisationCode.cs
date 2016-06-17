using System;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace SFA.DAS.OidcMiddleware.Builders
{
    public interface IBuildRequestAuthorisationCode
    {
        Task<TokenResponse> GetTokenResponse(string tokenEndpoint, string clientId, string clientSecret,string code, Uri redirectUri);
    }
}