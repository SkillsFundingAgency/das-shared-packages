using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Moq;
using SFA.DAS.GovUK.Auth.Authentication;

namespace SFA.DAS.GovUK.Auth.UnitTests.Authentication;

[TestFixture]
public class VerifiedIdentityFailureHandlerTests
{
    private VerifiedIdentityFailureHandler _handler;
    private AuthorizationPolicy _policy;

    [SetUp]
    public void SetUp()
    {
        _handler = new VerifiedIdentityFailureHandler();
        _policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new VerifiedIdentityRequirement())
            .Build();
    }

    [Test]
    public async Task Then_If_VerifiedRequirementFailed_Then_Redirect()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/secure/resource";
        httpContext.Request.QueryString = new QueryString("?x=1");

        var fakeHandler = Mock.Of<IAuthorizationHandler>();

        var failure = AuthorizationFailure.Failed(new[]
        {
            new AuthorizationFailureReason(fakeHandler, AuthorizationFailureMessages.NotVerified)
        });

        var result = PolicyAuthorizationResult.Forbid(failure);

        // Act
        var handled = await _handler.HandleFailureAsync(httpContext, _policy, result);

        // Assert
        handled.Should().BeTrue();
        httpContext.Response.Headers["Location"].ToString()
            .Should().Be("/service/verify-identity?returnUrl=%2Fsecure%2Fresource%3Fx%3D1");
    }

    [Test]
    public async Task Then_If_VerifiedRequirementNotInPolicy_Then_NotHandled()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        var result = PolicyAuthorizationResult.Forbid();

        // Act
        var handled = await _handler.HandleFailureAsync(httpContext, policy, result);

        // Assert
        handled.Should().BeFalse();
    }

    [Test]
    public async Task Then_If_FailureReason_Is_Not_NotVerified_Then_NotHandled()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        var fakeHandler = Mock.Of<IAuthorizationHandler>();

        var failure = AuthorizationFailure.Failed(new[]
        {
            new AuthorizationFailureReason(fakeHandler, "Some other failure")
        });

        var result = PolicyAuthorizationResult.Forbid(failure);

        // Act
        var handled = await _handler.HandleFailureAsync(httpContext, _policy, result);

        // Assert
        handled.Should().BeFalse();
    }
}
