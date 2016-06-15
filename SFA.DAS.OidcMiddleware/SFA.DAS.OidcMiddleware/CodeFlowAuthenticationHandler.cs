using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace SFA.DAS.OidcMiddleware
{
    public class CodeFlowAuthenticationHandler : AuthenticationHandler<OidcMiddlewareOptions>
    {
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            throw new NotImplementedException();
        }
    }


    public class OidcMiddlewareOptions : AuthenticationOptions
    {
        public OidcMiddlewareOptions(string authenticationType) : base(authenticationType)
        {
        }

        public string AuthorizeEndpoint { get; set; }
        public string Scopes { get; set; }
        public string ClientId { get; set; }
    }
}
