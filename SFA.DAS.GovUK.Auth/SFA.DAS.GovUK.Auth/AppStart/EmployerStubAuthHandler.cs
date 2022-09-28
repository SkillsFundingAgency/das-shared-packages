using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SFA.DAS.GovUK.Auth.AppStart;

internal class EmployerStubAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
        
    public EmployerStubAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            //TODO add claims that would come back as part of GOV.UK flow
            new Claim(ClaimTypes.Email, "testemployer@user.com")
        };
        var identity = new ClaimsIdentity(claims, "Employer-stub");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Employer-stub");
 
        var result = AuthenticateResult.Success(ticket);
 
        return Task.FromResult(result);
    }
}