using System;

namespace SFA.DAS.OidcMiddleware.Builders
{
    public interface IBuildAuthorizeRequestUrl
    {
        string GetAuthorizeRequestUrl(string authorityUrl, Uri requestUrl, string clientId, string scopes, string state, string nonce);
    }
}