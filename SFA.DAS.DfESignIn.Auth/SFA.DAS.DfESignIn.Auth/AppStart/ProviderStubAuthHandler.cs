using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using SFA.DAS.DfESignIn.Auth.Constants;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    internal class ProviderStubAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly ICustomClaims _customClaims;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProviderStubAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, IConfiguration configuration, ICustomClaims customClaims, IHttpContextAccessor httpContextAccessor) : base(
            options, logger, encoder, clock)
        {
            _configuration = configuration;
            _customClaims = customClaims;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return AuthenticateResult.Fail("");
            }

            if (!_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(StubAuthConstants.CookieName))
            {
                return AuthenticateResult.Fail("");
            }

            var authCookieValue = JsonConvert.DeserializeObject<StubAuthUserDetails>(_httpContextAccessor.HttpContext.Request.Cookies[StubAuthConstants.CookieName]);

            var claims = new List<Claim>
            {
                new Claim("sub", authCookieValue.Id),
                new Claim(ProviderClaims.DisplayName, authCookieValue.DisplayName),
                new Claim(ProviderClaims.Ukprn, authCookieValue.Ukprn),
            };

            foreach (var role in authCookieValue.Services.Split(' '))
            {
                claims.Add(new Claim(ProviderClaims.Service, role));
            }

            var identity = new ClaimsIdentity(claims, "Provider-stub");
            var principal = new ClaimsPrincipal(identity);

            if (_customClaims != null)
            {
                var additionalClaims = _customClaims.GetClaims(null);
                claims.AddRange(additionalClaims);
                principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Provider-stub"));
            }
            
            var ticket = new AuthenticationTicket(principal, "Provider-stub");

            var result = AuthenticateResult.Success(ticket);

            return result;
        }
    }
}