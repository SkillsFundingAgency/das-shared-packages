using System;
using IdentityModel.Client;

namespace SFA.DAS.OidcMiddleware
{
    public interface IBuildAuthorizeRequestUrl
    {
        string GetAuthorizeRequestUrl(string authorityUrl, Uri requestUrl, string clientId, string scopes, string state, string nonce);
    }
}