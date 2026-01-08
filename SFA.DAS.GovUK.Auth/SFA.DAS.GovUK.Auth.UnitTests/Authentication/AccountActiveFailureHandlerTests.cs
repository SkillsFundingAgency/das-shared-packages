using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.GovUK.Auth.Authentication;

namespace SFA.DAS.GovUK.Auth.UnitTests.Authentication;

[TestFixture]
public class AccountActiveFailureHandlerTests
{
    private DefaultHttpContext _httpContext;
    private AccountActiveFailureHandler _handler;
    private AuthorizationPolicy _policy;

    [SetUp]
    public void SetUp()
    {
        _httpContext = new DefaultHttpContext();
        _handler = new AccountActiveFailureHandler();
        _policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new AccountActiveRequirement())
            .Build();
    }

    [Test]
    public async Task Then_If_FailureReason_Is_NotAccountActive_Then_Redirects_To_Custom_Path()
    {
        var redirectUri = "https://service/suspended";
        var props = new AuthenticationProperties();
        props.Items["suspended_redirect"] = redirectUri;

        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(new ClaimsIdentity()), props, CookieAuthenticationDefaults.AuthenticationScheme);

        var authService = new Mock<IAuthenticationService>();
        authService.Setup(x => x.AuthenticateAsync(_httpContext, CookieAuthenticationDefaults.AuthenticationScheme))
            .ReturnsAsync(AuthenticateResult.Success(ticket));

        var services = new ServiceCollection()
            .AddSingleton(authService.Object)
            .BuildServiceProvider();

        _httpContext.RequestServices = services;

        var failure = AuthorizationFailure.Failed(new[]
        {
            new AuthorizationFailureReason(Mock.Of<IAuthorizationHandler>(), AuthorizationFailureMessages.NotAccountActive)
        });

        var result = PolicyAuthorizationResult.Forbid(failure);

        // Act
        var handled = await _handler.HandleFailureAsync(_httpContext, _policy, result);

        handled.Should().BeTrue();
        _httpContext.Response.Headers["Location"].ToString().Should().Be(redirectUri);
    }

    [Test]
    public async Task Then_If_No_Custom_Redirect_Path_In_Cookie_Then_Defaults_To_403()
    {
        var props = new AuthenticationProperties(); // No suspended_redirect
        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(new ClaimsIdentity()), props, CookieAuthenticationDefaults.AuthenticationScheme);

        var authService = new Mock<IAuthenticationService>();
        authService.Setup(x => x.AuthenticateAsync(_httpContext, CookieAuthenticationDefaults.AuthenticationScheme))
            .ReturnsAsync(AuthenticateResult.Success(ticket));

        var services = new ServiceCollection()
            .AddSingleton(authService.Object)
            .BuildServiceProvider();

        _httpContext.RequestServices = services;

        var failure = AuthorizationFailure.Failed(new[]
        {
            new AuthorizationFailureReason(Mock.Of<IAuthorizationHandler>(), AuthorizationFailureMessages.NotAccountActive)
        });

        var result = PolicyAuthorizationResult.Forbid(failure);

        var handled = await _handler.HandleFailureAsync(_httpContext, _policy, result);

        handled.Should().BeTrue();
        _httpContext.Response.Headers["Location"].ToString().Should().Be("/error/403");
    }

    [Test]
    public async Task Then_If_Requirement_Not_In_Policy_Then_NotHandled()
    {
        var otherPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        var result = PolicyAuthorizationResult.Forbid();

        var handled = await _handler.HandleFailureAsync(_httpContext, otherPolicy, result);

        handled.Should().BeFalse();
    }

    [Test]
    public async Task Then_If_FailureReason_Not_Matching_Then_NotHandled()
    {
        var failure = AuthorizationFailure.Failed(new[]
        {
            new AuthorizationFailureReason(Mock.Of<IAuthorizationHandler>(), "Different message")
        });

        var result = PolicyAuthorizationResult.Forbid(failure);

        var handled = await _handler.HandleFailureAsync(_httpContext, _policy, result);

        handled.Should().BeFalse();
    }
}
