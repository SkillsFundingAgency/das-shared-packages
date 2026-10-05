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
    private VerifiedIdentityFailureHandler _sut = null!;
    private AuthorizationPolicy _verifiedIdentityPolicy = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new VerifiedIdentityFailureHandler();

        _verifiedIdentityPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new VerifiedIdentityRequirement())
            .Build();
    }

    [Test]
    public async Task Then_If_Explain_Page_Is_Configured_Redirects_To_Explain_Page()
    {
        // Arrange
        _sut = new VerifiedIdentityFailureHandler("/explain-verify");

        var httpContext = CreateHttpContext();
        var result = CreateForbiddenResult(
            AuthorizationFailureMessages.NotVerified);

        // Act
        var handled = await _sut.HandleFailureAsync(
            httpContext,
            _verifiedIdentityPolicy,
            result);

        // Assert
        handled.Should().BeTrue();
        httpContext.Response.StatusCode.Should()
            .Be(StatusCodes.Status302Found);

        httpContext.Response.Headers.Location.ToString().Should().Be(
            "/explain-verify?returnUrl=%2Fsecure%2Fresource%3Fx%3D1");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public async Task Then_If_Explain_Page_Is_Not_Configured_Redirects_To_Default_Verify_Page(
        string? explainPageUrl)
    {
        // Arrange
        _sut = new VerifiedIdentityFailureHandler(explainPageUrl!);

        var httpContext = CreateHttpContext();
        var result = CreateForbiddenResult(
            AuthorizationFailureMessages.NotVerified);

        // Act
        var handled = await _sut.HandleFailureAsync(
            httpContext,
            _verifiedIdentityPolicy,
            result);

        // Assert
        handled.Should().BeTrue();
        httpContext.Response.StatusCode.Should()
            .Be(StatusCodes.Status302Found);

        httpContext.Response.Headers.Location.ToString().Should().Be(
            "/service/verify-identity?returnUrl=%2Fsecure%2Fresource%3Fx%3D1");
    }

    [Test]
    public async Task Then_If_Verified_Requirement_Is_Not_In_Policy_Does_Not_Handle()
    {
        // Arrange
        var httpContext = CreateHttpContext();

        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        var result = CreateForbiddenResult(
            AuthorizationFailureMessages.NotVerified);

        // Act
        var handled = await _sut.HandleFailureAsync(
            httpContext,
            policy,
            result);

        // Assert
        handled.Should().BeFalse();
        httpContext.Response.Headers.Location.Should().BeEmpty();
    }

    [Test]
    public async Task Then_If_Failure_Reason_Is_Not_Verified_Does_Not_Handle()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var result = CreateForbiddenResult("Some other failure");

        // Act
        var handled = await _sut.HandleFailureAsync(
            httpContext,
            _verifiedIdentityPolicy,
            result);

        // Assert
        handled.Should().BeFalse();
        httpContext.Response.Headers.Location.Should().BeEmpty();
    }

    [Test]
    public async Task Then_If_Authorization_Failure_Is_Missing_Does_Not_Handle()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var result = PolicyAuthorizationResult.Forbid();

        // Act
        var handled = await _sut.HandleFailureAsync(
            httpContext,
            _verifiedIdentityPolicy,
            result);

        // Assert
        handled.Should().BeFalse();
        httpContext.Response.Headers.Location.Should().BeEmpty();
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var httpContext = new DefaultHttpContext();

        httpContext.Request.Path = "/secure/resource";
        httpContext.Request.QueryString = new QueryString("?x=1");

        return httpContext;
    }

    private static PolicyAuthorizationResult CreateForbiddenResult(
        string failureReason)
    {
        var authorizationHandler = Mock.Of<IAuthorizationHandler>();

        var failure = AuthorizationFailure.Failed(new[]
        {
            new AuthorizationFailureReason(
                authorizationHandler,
                failureReason)
        });

        return PolicyAuthorizationResult.Forbid(failure);
    }
}