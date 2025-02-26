using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests.Authentication;

public class TestAuthHandler : AuthenticationHandler<TestAuthenticationOptions>
{
    public TestAuthHandler(IOptionsMonitor<TestAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // if the header is provided and its value is 'true', then step aside and let the regular auth handle it
        // a value of <anything other than 'true'> will bypass the rest of the auth pipeline
        // if (!Context.Request.Headers.TryGetValue("X-Test-Auth", out var header) || header.Contains("true"))
        // {
        //     return Task.FromResult(AuthenticateResult.NoResult());
        // }
        // value is false (or anything other than 'true'). Make sure they've made the effort to provide the auth header 
        // but don't bother to check the value
        // if (header.Any() && !Context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        // {
        //     return Task.FromResult(AuthenticateResult.NoResult());
        // }
        // otherwise, here's your auth ticket!
        return Task.FromResult(
            AuthenticateResult.Success(
                new AuthenticationTicket(
                    new ClaimsPrincipal(Options.Identity),
                    new AuthenticationProperties(),
                    Scheme.Name)));

    }
}