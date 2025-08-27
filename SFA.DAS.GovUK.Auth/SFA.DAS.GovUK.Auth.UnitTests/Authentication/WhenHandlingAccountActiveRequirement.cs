using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.Testing.AutoFixture;
using System.Security.Claims;

namespace SFA.DAS.GovUK.Auth.UnitTests.Authentication;

public class WhenHandlingAccountActiveRequirement
{
    private Mock<HttpContext> _mockHttpContext;
    private Mock<HttpResponse> _mockHttpResponse;

    [SetUp]
    public void Arrange()
    {
        _mockHttpContext = new Mock<HttpContext>();
        _mockHttpResponse = new Mock<HttpResponse>();
        _mockHttpContext.Setup(x => x.Response).Returns(_mockHttpResponse.Object);

        // AuthenticateAsync returns properties with suspended redirect url
        var props = new AuthenticationProperties();
        props.Items["suspended_redirect"] = "https://myservice/suspended";
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity()), props,
            CookieAuthenticationDefaults.AuthenticationScheme);
        var authService = new Mock<IAuthenticationService>();
        authService
            .Setup(s => s.AuthenticateAsync(_mockHttpContext.Object, CookieAuthenticationDefaults.AuthenticationScheme))
            .ReturnsAsync(AuthenticateResult.Success(ticket));

        // IAuthenticationService is returned from RequestServices
        var services = new ServiceCollection()
            .AddSingleton(authService.Object)
            .BuildServiceProvider();
        _mockHttpContext.Setup(x => x.RequestServices).Returns(services);
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_Claim_Does_Not_Exist_Then_Succeeds(
        AccountActiveRequirement requirement,
        AccountActiveAuthorizationHandler authorizationHandler)
    {
        // Arrange
        var suspended = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("AccountSuspended", "True")]));
        var context = new AuthorizationHandlerContext([requirement], suspended, _mockHttpContext.Object);

        // Act
        await authorizationHandler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
        _mockHttpResponse.Verify(x => x.Redirect("https://myservice/suspended"), Times.Never);
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_Claim_Does_Exist_And_Not_Suspended_Then_Succeeds(
        AccountActiveRequirement requirement,
        AccountActiveAuthorizationHandler authorizationHandler)
    {
        // Arrange
        var suspended = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.AuthorizationDecision, "Active")]));
        var context = new AuthorizationHandlerContext([requirement], suspended, _mockHttpContext.Object);

        // Act
        await authorizationHandler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
        _mockHttpResponse.Verify(x=>x.Redirect("https://myservice/suspended"), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_If_Claim_Does_Exist_And_Is_Suspended_Then_Succeeds_And_Redirects(
        AccountActiveRequirement requirement,
        AccountActiveAuthorizationHandler authorizationHandler)
    {
        // Arrange
        var suspended = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.AuthorizationDecision, AuthorizationDecisions.Suspended)]));
        var ctx = new AuthorizationHandlerContext([requirement], suspended, _mockHttpContext.Object);

        // Act
        await authorizationHandler.HandleAsync(ctx);

        // Assert
        _mockHttpResponse.Verify(r => r.Redirect("https://myservice/suspended"));
    }

    [Test, MoqAutoData]
    public async Task Then_If_Claim_Does_Exist_And_Is_Suspended_Then_Succeeds_And_Redirects_For_Filter_Context(
        AccountActiveRequirement requirement,
        AccountActiveAuthorizationHandler authorizationHandler)
    {
        // Arrange
        var suspended = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.AuthorizationDecision, AuthorizationDecisions.Suspended)]));
        var filterContext = new AuthorizationFilterContext(new ActionContext(_mockHttpContext.Object, new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>());
        var context = new AuthorizationHandlerContext([requirement], suspended, filterContext);

        // Act
        await authorizationHandler.HandleAsync(context);

        // Assert
        _mockHttpResponse.Verify(r => r.Redirect("https://myservice/suspended"));
    }
}