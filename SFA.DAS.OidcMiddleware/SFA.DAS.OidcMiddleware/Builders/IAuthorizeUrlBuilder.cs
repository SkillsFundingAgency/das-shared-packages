using System;

namespace SFA.DAS.OidcMiddleware.Builders
{
    public interface IAuthorizeUrlBuilder
    {
        string BuildAuthorizeUrl(string authorizeEndpoint, string clientId, string scopes, Uri requestUri, string state, string nonce);
    }
}