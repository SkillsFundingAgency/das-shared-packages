using System;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;

namespace SFA.DAS.OidcMiddleware
{
    public class OidcMiddlewareOptions : AuthenticationOptions
    {
        public OidcMiddlewareOptions(string authenticationType) : base(authenticationType)
        {
        }
        public OidcMiddlewareOptions() : base("code")
        {

        }

        public string AuthorizeEndpoint { get; set; }
        public string Scopes { get; set; }
        public string ClientId { get; set; }

        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
        public string ClientSecret { get; set; }
        public string TokenEndpoint { get; set; }
        public string BaseUrl { get; set; }
        public string UserInfoEndpoint { get; set; }
        public Action<ClaimsIdentity> AuthenticatedCallback { get; set; }
    }
}