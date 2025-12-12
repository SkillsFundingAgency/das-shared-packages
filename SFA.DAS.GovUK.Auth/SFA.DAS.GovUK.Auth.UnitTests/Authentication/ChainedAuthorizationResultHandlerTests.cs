using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.GovUK.Auth.Authentication;

namespace SFA.DAS.GovUK.Auth.UnitTests.Authentication;

[TestFixture]
public class ChainedAuthorizationResultHandlerTests
{
    private DefaultHttpContext _httpContext;
    private RequestDelegate _next;

    [SetUp]
    public void SetUp()
    {
        _httpContext = new DefaultHttpContext();
        _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "user") }, "mock"));

        _next = context => Task.CompletedTask;
    }

    [Test]
    public async Task Then_Calls_First_Handler_That_Handles_Failure()
    {
        // Arrange
        var handler1 = new Mock<IAuthorizationFailureHandler>();
        var handler2 = new Mock<IAuthorizationFailureHandler>();

        handler1.Setup(h => h.HandleFailureAsync(_httpContext, It.IsAny<AuthorizationPolicy>(), It.IsAny<PolicyAuthorizationResult>()))
            .ReturnsAsync(false);

        handler2.Setup(h => h.HandleFailureAsync(_httpContext, It.IsAny<AuthorizationPolicy>(), It.IsAny<PolicyAuthorizationResult>()))
            .ReturnsAsync(true);

        var chainedHandler = new ChainedAuthorizationResultHandler(new[] { handler1.Object, handler2.Object });

        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        var result = PolicyAuthorizationResult.Forbid();

        // Act
        await chainedHandler.HandleAsync(_next, _httpContext, policy, result);

        // Assert
        handler1.Verify(h => h.HandleFailureAsync(_httpContext, policy, result), Times.Once);
        handler2.Verify(h => h.HandleFailureAsync(_httpContext, policy, result), Times.Once);
    }

    [Test]
    public async Task Then_Skips_Handler_Chain_If_Authorization_Succeeded()
    {
        // Arrange
        var handler = new Mock<IAuthorizationFailureHandler>();
        var chainedHandler = new ChainedAuthorizationResultHandler(new[] { handler.Object });

        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        var result = PolicyAuthorizationResult.Success();

        // Act
        await chainedHandler.HandleAsync(_next, _httpContext, policy, result);

        // Assert
        handler.Verify(h => h.HandleFailureAsync(It.IsAny<HttpContext>(), It.IsAny<AuthorizationPolicy>(), It.IsAny<PolicyAuthorizationResult>()), Times.Never);
    }

    [Test]
    public async Task Then_Uses_Default_Handler_If_No_Custom_Handlers_Handle()
    {
        // Arrange
        var handler = new Mock<IAuthorizationFailureHandler>();
        handler.Setup(h => h.HandleFailureAsync(It.IsAny<HttpContext>(), It.IsAny<AuthorizationPolicy>(), It.IsAny<PolicyAuthorizationResult>()))
               .ReturnsAsync(false);

        var chainedHandler = new ChainedAuthorizationResultHandler(new[] { handler.Object });

        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        var result = PolicyAuthorizationResult.Success();

        var context = new DefaultHttpContext();
        context.User = _httpContext.User;

        var services = new ServiceCollection();

        services
            .AddAuthentication("TestScheme")
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                "TestScheme", options => { });

        var serviceProvider = services.BuildServiceProvider();
        context.RequestServices = serviceProvider;

        var wasCalled = false;
        var next = new RequestDelegate(_ =>
        {
            wasCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await chainedHandler.HandleAsync(next, context, policy, result);

        // Assert
        wasCalled.Should().BeTrue();
    }


    [Test]
    public async Task Then_Does_Not_Invoke_Handlers_If_User_Is_Not_Authenticated()
    {
        // Arrange
        var handler = new Mock<IAuthorizationFailureHandler>();
        var chainedHandler = new ChainedAuthorizationResultHandler(new[] { handler.Object });

        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        var result = PolicyAuthorizationResult.Forbid();

        var context = new DefaultHttpContext();
        
        var services = new ServiceCollection();
        services
            .AddAuthentication("TestScheme")
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                "TestScheme", options => { });

        var serviceProvider = services.BuildServiceProvider();
        context.RequestServices = serviceProvider;

        // Act
        await chainedHandler.HandleAsync(_next, context, policy, result);

        // Assert
        handler.Verify(h => h.HandleFailureAsync(It.IsAny<HttpContext>(), It.IsAny<AuthorizationPolicy>(), It.IsAny<PolicyAuthorizationResult>()), Times.Never);
    }
}
