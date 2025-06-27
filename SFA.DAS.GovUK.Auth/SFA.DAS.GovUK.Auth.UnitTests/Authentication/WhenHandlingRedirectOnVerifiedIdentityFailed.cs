using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.GovUK.Auth.Authentication;

namespace SFA.DAS.GovUK.Auth.UnitTests.Authentication
{
    [TestFixture]
    public class WhenHandlingRedirectOnVerifiedIdentityFailed
    {
        private RedirectOnVerifiedIdentityFailedResultHandler _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new RedirectOnVerifiedIdentityFailedResultHandler();
        }

        [Test]
        public async Task Then_If_VerifiedIdentityRequirement_Failed_And_Response_Is_302_DefaultHandler_Is_Not_Called()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.StatusCode = StatusCodes.Status302Found;

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new VerifiedIdentityRequirement())
                .Build();

            var authorizeResult = PolicyAuthorizationResult.Forbid();

            var nextCalled = false;
            RequestDelegate next = _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            // Act
            await _sut.HandleAsync(next, context, policy, authorizeResult);

            // Assert
            nextCalled.Should().BeFalse("next delegate should not be called");
            context.Response.StatusCode.Should().Be(StatusCodes.Status302Found);
        }

        [TestCase(StatusCodes.Status200OK)]
        [TestCase(StatusCodes.Status403Forbidden)]
        [TestCase(StatusCodes.Status404NotFound)]
        public async Task Then_If_Response_Is_Not_302_And_Requirement_Fails_DefaultHandler_Forbids(int statusCode)
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.StatusCode = statusCode;

            var authServiceMock = new Mock<IAuthenticationService>();
            var services = new ServiceCollection();
            services.AddSingleton(authServiceMock.Object);
            context.RequestServices = services.BuildServiceProvider();

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new VerifiedIdentityRequirement())
                .Build();

            var authorizeResult = PolicyAuthorizationResult.Forbid();

            var next = new RequestDelegate(_ => Task.CompletedTask);

            // Act
            await _sut.HandleAsync(next, context, policy, authorizeResult);

            // Assert:
            authServiceMock.Verify(a =>
                a.ForbidAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(), // scheme
                    It.IsAny<AuthenticationProperties>()), Times.Once);
        }


        [Test]
        public async Task Then_If_VerifiedIdentityRequirement_Is_Not_In_Policy_DefaultHandler_Is_Called()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.StatusCode = StatusCodes.Status302Found;

            var authServiceMock = new Mock<IAuthenticationService>();
            var services = new ServiceCollection();
            services.AddSingleton(authServiceMock.Object);
            context.RequestServices = services.BuildServiceProvider();

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new DenyAnonymousAuthorizationRequirement())
                .Build();

            var authorizeResult = PolicyAuthorizationResult.Forbid();

            var next = new RequestDelegate(_ => Task.CompletedTask);

            // Act
            await _sut.HandleAsync(next, context, policy, authorizeResult);

            // Assert
            authServiceMock.Verify(a =>
                a.ForbidAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(), // scheme
                    It.IsAny<AuthenticationProperties>()), Times.Once);
        }

        [Test]
        public async Task Then_If_Authorization_Succeeds_DefaultHandler_Is_Called()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.StatusCode = StatusCodes.Status302Found;

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new VerifiedIdentityRequirement())
                .Build();

            var authorizeResult = PolicyAuthorizationResult.Success();

            var nextCalled = false;
            RequestDelegate next = _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            // Act
            await _sut.HandleAsync(next, context, policy, authorizeResult);

            // Assert
            nextCalled.Should().BeTrue("should call default handler when authorization succeeds");
        }
    }
}
