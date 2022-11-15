using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.AppStart;

internal class EmployerStubAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;
    private readonly ICustomClaims? _customClaims;

    public EmployerStubAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration configuration, ICustomClaims? customClaims) : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
        _customClaims = customClaims;
    }
    
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, _configuration["NoAuthEmail"]),
            new Claim(ClaimTypes.NameIdentifier, Guid.Empty.ToString()),
            new Claim("sub", Guid.Empty.ToString())
        };

        if (_customClaims != null)
        {
            var additionalClaims = await _customClaims.GetClaims(null!);
            claims.AddRange(additionalClaims!);
        }
        
        var identity = new ClaimsIdentity(claims, "Employer-stub");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Employer-stub");
 
        var result = AuthenticateResult.Success(ticket);
 
        return result;
    }
}