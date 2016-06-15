using System;
using IdentityModel.Client;

namespace SFA.DAS.OidcMiddleware
{
    public class BuildAuthorizeRequestUrl : IBuildAuthorizeRequestUrl
    {
        public string GetAuthorizeRequestUrl(string authorityUrl, Uri requestUrl, string clientId, string scopes, string state, string nonce)
        {
            var request =  new AuthorizeRequest(authorityUrl);
            
            return request.CreateAuthorizeUrl(
                clientId: clientId,
                responseType: "code",
                scope: scopes,
                redirectUri: $"{requestUrl.Scheme}{Uri.SchemeDelimiter}{requestUrl.Authority}{requestUrl.AbsolutePath}",
                state: state,
                nonce: nonce);
        }
    }
}