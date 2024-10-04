using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.DfESignIn.Auth.Models;
using Microsoft.AspNetCore.Http;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using Microsoft.Extensions.Configuration;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Constants;

namespace SFA.DAS.DfESignIn.Auth.Services
{
    public interface IStubAuthenticationService
    {
        ClaimsPrincipal GetStubSignInClaims(StubAuthUserDetails model);
    }

    public class StubAuthenticationService : IStubAuthenticationService
    {
        private readonly ICustomClaims _customClaims;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _environment;

        public StubAuthenticationService(IConfiguration configuration, ICustomClaims customClaims, IHttpContextAccessor httpContextAccessor)
        {
            _customClaims = customClaims;
            _httpContextAccessor = httpContextAccessor;
            _environment = configuration["ResourceEnvironmentName"]?.ToUpper();
        }

        public ClaimsPrincipal GetStubSignInClaims(StubAuthUserDetails model)
        {
            if (_environment == "PRD")
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim("sub", model.Id),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.NameIdentifier, model.Id),
                new Claim(ProviderClaims.Ukprn, model.Ukprn),
            };
            foreach (var role in model.Services.Split(' '))
            {
                claims.Add(new Claim(ProviderClaims.Service, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            if (_customClaims != null)
            {
                var additionalClaims = _customClaims.GetClaims(new TokenValidatedContext(_httpContextAccessor.HttpContext, new AuthenticationScheme(CookieAuthenticationDefaults.AuthenticationScheme, "Cookie", typeof(ProviderStubAuthHandler)), new OpenIdConnectOptions(), principal, new AuthenticationProperties()));
                claims.AddRange(additionalClaims);
                principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            }

            return principal;
        }
    }
}
