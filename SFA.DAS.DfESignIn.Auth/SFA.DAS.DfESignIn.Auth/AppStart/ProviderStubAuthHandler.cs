using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    internal class ProviderStubAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration,
        ICustomClaims customClaims,
        IHttpContextAccessor httpContextAccessor)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "10000001"),
                new Claim("http://schemas.portal.com/displayname", "APIM Provider User"),
                new Claim("http://schemas.portal.com/service", "DAA"),
                new Claim("http://schemas.portal.com/ukprn", "10000001")
            };

            if (customClaims != null)
            {
                var additionalClaims = customClaims.GetClaims(null);
                claims.AddRange(additionalClaims);
            }

            var identity = new ClaimsIdentity(claims, "Provider-stub");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Provider-stub");

            var result = AuthenticateResult.Success(ticket);

            httpContextAccessor.HttpContext.Items.Add("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "10000001");
            httpContextAccessor.HttpContext.Items.Add("http://schemas.portal.com/displayname", "APIM Provider User");

            return Task.FromResult(result);
        }
    }
}