using System;
using IdentityModel.Client;

namespace SFA.DAS.OidcMiddleware.Builders
{
    public class AuthorizeUrlBuilder : IAuthorizeUrlBuilder
    {
        public string BuildAuthorizeUrl(string authorizeEndpoint, string clientId, string scopes, Uri requestUri, string state, string nonce)
        {
            var request = new RequestUrl(authorizeEndpoint);
            
            return request.CreateAuthorizeUrl(
                clientId: clientId,
                responseType: "code",
                scope: scopes,
                redirectUri: $"{requestUri.Scheme}{Uri.SchemeDelimiter}{requestUri.Authority}{requestUri.AbsolutePath}",
                state: state,
                nonce: nonce);
        }
    }
}