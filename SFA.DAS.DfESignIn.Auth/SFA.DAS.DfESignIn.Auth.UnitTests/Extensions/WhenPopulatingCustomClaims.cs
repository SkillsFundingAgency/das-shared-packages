using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using System.Security.Claims;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Extensions
{
    public class WhenPopulatingCustomClaims
    {
        private Mock<ICustomClaims> _customClaims = null!;
        private ServiceProvider _serviceProvider = null!;
        private IServiceScope _scope = null!;
        private TokenValidatedContext _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _customClaims = new Mock<ICustomClaims>();

            _customClaims
                .Setup(handler => handler.GetClaimsAsync(
                    It.IsAny<TokenValidatedContext>()))
                .ReturnsAsync(Array.Empty<Claim>());

            _serviceProvider = new ServiceCollection()
                .AddScoped<ICustomClaims>(
                    _ => _customClaims.Object)
                .BuildServiceProvider();

            _scope = _serviceProvider.CreateScope();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                new[]
                {
                    new Claim("sub", "user-one")
                },
                CookieAuthenticationDefaults.AuthenticationScheme));

            _sut = new TokenValidatedContext(
                new DefaultHttpContext
                {
                    RequestServices = _scope.ServiceProvider
                },
                new AuthenticationScheme(
                    OpenIdConnectDefaults.AuthenticationScheme,
                    "DfE Sign-in",
                    typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions
                {
                    ClientId = "assessor-client"
                },
                principal,
                new AuthenticationProperties());
        }

        [TearDown]
        public void TearDown()
        {
            _scope.Dispose();
            _serviceProvider.Dispose();
        }

        [Test]
        public async Task Then_The_Handler_Is_Awaited_And_Its_Claims_Are_Added()
        {
            // Arrange
            var completion = new TaskCompletionSource<IEnumerable<Claim>>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            _customClaims
                .Setup(handler => handler.GetClaimsAsync(_sut))
                .Returns(completion.Task);

            // Act
            var task = _sut.PopulateCustomClaimsAsync();

            var wasWaitingForClaims = !task.IsCompleted;

            completion.SetResult(new[]
            {
                new Claim("application-user-id", "application-user-one")
            });

            await task;

            // Assert
            wasWaitingForClaims.Should().BeTrue();

            _sut.Principal!.FindFirst("sub")!.Value
                .Should().Be("user-one");

            _sut.Principal.FindFirst("application-user-id")!.Value
                .Should().Be("application-user-one");

            _customClaims.Verify(
                handler => handler.GetClaimsAsync(_sut),
                Times.Once);
        }

        [Test]
        public async Task Then_No_Handler_Leaves_The_Principal_Unchanged()
        {
            // Arrange
            using var provider = new ServiceCollection()
                .BuildServiceProvider();

            _sut.HttpContext.RequestServices = provider;

            var originalPrincipal = _sut.Principal;

            // Act
            await _sut.PopulateCustomClaimsAsync();

            // Assert
            _sut.Principal.Should().BeSameAs(originalPrincipal);
            _sut.Principal!.Claims.Should().ContainSingle();

            _customClaims.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Then_A_Replacement_Principal_Receives_The_Claims()
        {
            // Arrange
            var replacement = new ClaimsPrincipal(new ClaimsIdentity(
                CookieAuthenticationDefaults.AuthenticationScheme));

            _customClaims
                .Setup(handler => handler.GetClaimsAsync(_sut))
                .Callback<TokenValidatedContext>(
                    context => context.Principal = replacement)
                .ReturnsAsync(new[]
                {
                    new Claim("application-user-id", "application-user-one")
                });

            // Act
            await _sut.PopulateCustomClaimsAsync();

            // Assert
            _sut.Principal.Should().BeSameAs(replacement);

            replacement.FindFirst("application-user-id")!.Value
                .Should().Be("application-user-one");
        }

        [TestCase("handled")]
        [TestCase("skipped")]
        [TestCase("failed")]
        public async Task Then_Stopping_Authentication_Prevents_Additional_Claims(
            string outcome)
        {
            // Arrange
            _customClaims
                .Setup(handler => handler.GetClaimsAsync(_sut))
                .Callback<TokenValidatedContext>(context =>
                {
                    switch (outcome)
                    {
                        case "handled":
                            context.HandleResponse();
                            break;

                        case "skipped":
                            context.SkipHandler();
                            break;

                        case "failed":
                            context.Fail("Access denied.");
                            break;
                    }
                })
                .ReturnsAsync(new[]
                {
                    new Claim("application-user-id", "application-user-one")
                });

            // Act
            await _sut.PopulateCustomClaimsAsync();

            // Assert
            _sut.Principal!.FindFirst("application-user-id")
                .Should().BeNull();

            _sut.Result.Should().NotBeNull();
        }

        [Test]
        public async Task Then_Handler_Exceptions_Are_Propagated()
        {
            // Arrange
            _customClaims
                .Setup(handler => handler.GetClaimsAsync(_sut))
                .ThrowsAsync(new InvalidOperationException(
                    "User creation failed."));

            // Act
            Func<Task> act = () => _sut.PopulateCustomClaimsAsync();

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("User creation failed.");
        }

        [Test]
        public async Task Then_The_Ticket_Receives_The_Enriched_Principal()
        {
            // Arrange
            var replacement = new ClaimsPrincipal(new ClaimsIdentity(
                CookieAuthenticationDefaults.AuthenticationScheme));

            _customClaims
                .Setup(handler => handler.GetClaimsAsync(
                    It.IsAny<TokenValidatedContext>()))
                .Callback<TokenValidatedContext>(
                    context => context.Principal = replacement)
                .ReturnsAsync(new[]
                {
                    new Claim("application-user-id", "application-user-one")
                });

            var ticketContext = CreateTicketContext();

            // Act
            await ticketContext.PopulateCustomClaimsAsync();

            // Assert
            ticketContext.Principal.Should().BeSameAs(replacement);

            ticketContext.Principal!.FindFirst("application-user-id")!.Value
                .Should().Be("application-user-one");
        }

        [TestCase("handled")]
        [TestCase("skipped")]
        [TestCase("failed")]
        public async Task Then_The_Handler_Result_Is_Transferred_To_The_Ticket(
            string outcome)
        {
            // Arrange
            _customClaims
                .Setup(handler => handler.GetClaimsAsync(
                    It.IsAny<TokenValidatedContext>()))
                .Callback<TokenValidatedContext>(context =>
                {
                    switch (outcome)
                    {
                        case "handled":
                            context.HandleResponse();
                            break;

                        case "skipped":
                            context.SkipHandler();
                            break;

                        case "failed":
                            context.Fail("Access denied.");
                            break;
                    }
                })
                .ReturnsAsync(Array.Empty<Claim>());

            var ticketContext = CreateTicketContext();

            // Act
            await ticketContext.PopulateCustomClaimsAsync();

            // Assert
            ticketContext.Result.Should().NotBeNull();

            switch (outcome)
            {
                case "handled":
                    ticketContext.Result!.Handled.Should().BeTrue();
                    break;

                case "skipped":
                    ticketContext.Result!.Skipped.Should().BeTrue();
                    break;

                case "failed":
                    ticketContext.Result!.Failure.Should().NotBeNull();
                    ticketContext.Result.Failure!.Message
                        .Should().Be("Access denied.");
                    break;
            }
        }

        private TicketReceivedContext CreateTicketContext()
        {
            return new TicketReceivedContext(
                _sut.HttpContext,
                _sut.Scheme,
                _sut.Options,
                new AuthenticationTicket(
                    _sut.Principal!,
                    _sut.Properties,
                    _sut.Scheme.Name));
        }
    }
}