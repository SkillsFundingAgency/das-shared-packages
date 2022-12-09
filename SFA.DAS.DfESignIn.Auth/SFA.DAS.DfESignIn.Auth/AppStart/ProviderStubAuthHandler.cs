using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    internal class ProviderStubAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly ICustomClaims _customClaims;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProviderStubAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, IConfiguration configuration, ICustomClaims customClaims) : base(
            options, logger, encoder, clock)
        {
            _configuration = configuration;
            _customClaims = customClaims;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "10000001"),
                new Claim("http://schemas.portal.com/displayname", "APIM Provider User"),
                new Claim("http://schemas.portal.com/service", "DAA"),
                new Claim("http://schemas.portal.com/ukprn", "10000001")
            };

            if (_customClaims != null)
            {
                var additionalClaims = _customClaims.GetClaims(null);
                claims.AddRange(additionalClaims);
            }

            var identity = new ClaimsIdentity(claims, "Provider-stub");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Provider-stub");

            var result = AuthenticateResult.Success(ticket);

            _httpContextAccessor.HttpContext.Items.Add("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "10000001");
            _httpContextAccessor.HttpContext.Items.Add("http://schemas.portal.com/displayname", "APIM Provider User");

            return result;
        }
    }
}