using System;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin.Security;
using SFA.DAS.OidcMiddleware.Builders;
using SFA.DAS.OidcMiddleware.Caches;
using SFA.DAS.OidcMiddleware.Clients;
using SFA.DAS.OidcMiddleware.Validators;

namespace SFA.DAS.OidcMiddleware
{
    public class OidcMiddlewareOptions : AuthenticationOptions
    {
        public OidcMiddlewareOptions(string authenticationType)
            : base(authenticationType)
        {
        }

        public OidcMiddlewareOptions()
            : base("code")
        {
        }

        public Action<ClaimsIdentity> AuthenticatedCallback { get; set; }
        public string AuthorizeEndpoint { get; set; }
        public IAuthorizeUrlBuilder AuthorizeUrlBuilder { get; set; }
        public INonceCache NonceCache { get; set; }
        public ITokenClient TokenClient { get; set; }
        public ITokenValidator TokenValidator { get; set; }
        public IUserInfoClient UserInfoClient { get; set; }
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scopes { get; set; }
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
        public string TokenEndpoint { get; set; }
        public Func<X509Certificate2> TokenSigningCertificateLoader { get; set; }
        public TokenValidationMethod TokenValidationMethod { get; set; } = TokenValidationMethod.BinarySecret;
        public string UserInfoEndpoint { get; set; }
    }
}