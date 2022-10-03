using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SFA.DAS.GovUK.Auth.AppStart;

internal class EmployerStubAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;
    public EmployerStubAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration configuration) : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, _configuration["NoAuthEmail"]),
            new Claim(ClaimTypes.NameIdentifier, Guid.Empty.ToString()),
            new Claim("sub", Guid.Empty.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Employer-stub");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Employer-stub");
 
        var result = AuthenticateResult.Success(ticket);
 
        return Task.FromResult(result);
    }
}