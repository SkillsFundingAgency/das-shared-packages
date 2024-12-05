using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.AppStart;

internal class EmployerStubAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock,
    ICustomClaims customClaims,
    IHttpContextAccessor httpContextAccessor)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(GovUkConstants.StubAuthCookieName))
        {
            return AuthenticateResult.Fail("");
        }

        var authCookieValue = JsonConvert.DeserializeObject<StubAuthUserDetails>(httpContextAccessor.HttpContext.Request.Cookies[GovUkConstants.StubAuthCookieName]);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, authCookieValue.Email),
            new(ClaimTypes.NameIdentifier, authCookieValue.Id),
            new("sub", authCookieValue.Id)
        };
        
        if (authCookieValue.Mobile != null)
        {
            claims.Add(new Claim(ClaimTypes.MobilePhone, authCookieValue.Mobile));
        }

        var identity = new ClaimsIdentity(claims, "Employer-stub");
        var principal = new ClaimsPrincipal(identity);
            
        if (customClaims != null)
        {
            var additionalClaims = await customClaims.GetClaims(new TokenValidatedContext(httpContextAccessor.HttpContext, Scheme, new OpenIdConnectOptions(), principal, new AuthenticationProperties() ));
            claims.AddRange(additionalClaims);
            principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Employer-stub"));
        }            

        var ticket = new AuthenticationTicket(principal, "Employer-stub");

            
        var result = AuthenticateResult.Success(ticket);

        return result;
    }
}