using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.UnitTests.AppStart
{
    [TestFixture]
    public class GovUkCookieAuthenticationEventsTests
    {
        private GovUkOidcConfiguration _config;
        private Mock<IOptions<GovUkOidcConfiguration>> _configMock;
        private Mock<ITicketStore> _ticketStoreMock;
        private GovUkCookieAuthenticationEvents _sut;

        [SetUp]
        public void SetUp()
        {
            _config = new GovUkOidcConfiguration();
            _configMock = new Mock<IOptions<GovUkOidcConfiguration>>();
            _configMock.Setup(x => x.Value).Returns(_config);

            _ticketStoreMock = new Mock<ITicketStore>();

            _sut = new GovUkCookieAuthenticationEvents(
                _configMock.Object,
                _ticketStoreMock.Object);
        }

        [Test]
        public async Task ValidatePrincipal_Updates_Vot_When_Verify_Enabled_By_Properties()
        {
            // Arrange
            _config.EnableVerify = null;

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                GovUkUserClaimTypes.VerifiedIdentity, "true"));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));

            var validateContext = CreateContext(claimsIdentity, true);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm.P2");
            claimsIdentity.FindAll("vot").Should().ContainSingle();
            validateContext.ShouldRenew.Should().BeTrue();

            _ticketStoreMock.Verify(x => x.RenewAsync(
                "session-abc",
                It.Is<AuthenticationTicket>(ticket =>
                    ticket.Principal.HasClaim("vot", "Cl.Cm.P2"))),
                Times.Once);
        }

        [Test]
        public async Task ValidatePrincipal_Adds_Vot_When_Verify_Enabled_By_Config()
        {
            // Arrange
            _config.EnableVerify = "true";

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                GovUkUserClaimTypes.VerifiedIdentity, "true"));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm.P2");
            validateContext.ShouldRenew.Should().BeTrue();

            _ticketStoreMock.Verify(x => x.RenewAsync(
                "session-abc",
                It.IsAny<AuthenticationTicket>()),
                Times.Once);
        }

        [Test]
        public async Task ValidatePrincipal_DoesNothing_When_Verify_Not_Enabled()
        {
            // Arrange
            _config.EnableVerify = null;

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                GovUkUserClaimTypes.VerifiedIdentity, "true"));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));

            var validateContext = CreateContext(claimsIdentity, false);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm");
            validateContext.ShouldRenew.Should().BeFalse();

            _ticketStoreMock.Verify(x => x.RenewAsync(
                It.IsAny<string>(),
                It.IsAny<AuthenticationTicket>()),
                Times.Never);
        }

        [Test]
        public async Task ValidatePrincipal_DoesNothing_When_VerifiedIdentity_Claim_Is_Missing()
        {
            // Arrange
            _config.EnableVerify = "true";

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                GovUkUserClaimTypes.UserInfo, "{}"));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm");
            validateContext.ShouldRenew.Should().BeFalse();

            _ticketStoreMock.Verify(x => x.RenewAsync(
                It.IsAny<string>(),
                It.IsAny<AuthenticationTicket>()),
                Times.Never);
        }

        [Test]
        public async Task ValidatePrincipal_DoesNothing_When_VerifiedIdentity_Is_False()
        {
            // Arrange
            _config.EnableVerify = "true";

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                GovUkUserClaimTypes.VerifiedIdentity, "false"));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm");
            validateContext.ShouldRenew.Should().BeFalse();

            _ticketStoreMock.Verify(x => x.RenewAsync(
                It.IsAny<string>(),
                It.IsAny<AuthenticationTicket>()),
                Times.Never);
        }

        [Test]
        public async Task ValidatePrincipal_DoesNothing_When_Vot_Already_Correct()
        {
            // Arrange
            _config.EnableVerify = "true";

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                GovUkUserClaimTypes.VerifiedIdentity, "true"));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm.P2"));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm.P2");
            validateContext.ShouldRenew.Should().BeFalse();

            _ticketStoreMock.Verify(x => x.RenewAsync(
                It.IsAny<string>(),
                It.IsAny<AuthenticationTicket>()),
                Times.Never);
        }

        [Test]
        public async Task ValidatePrincipal_DoesNot_Change_Existing_Name_Claims()
        {
            // Arrange
            _config.EnableVerify = "true";

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                GovUkUserClaimTypes.VerifiedIdentity, "true"));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, "Jane Smith"));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, "Jane"));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Surname, "Smith"));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm.P2");
            claimsIdentity.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Jane Smith");
            claimsIdentity.FindFirst(ClaimTypes.GivenName)!.Value.Should().Be("Jane");
            claimsIdentity.FindFirst(ClaimTypes.Surname)!.Value.Should().Be("Smith");
        }

        private CookieValidatePrincipalContext CreateContext(
            ClaimsIdentity claimsIdentity,
            bool? enableVerifyProperty)
        {
            var principal = new ClaimsPrincipal(claimsIdentity);

            var items = new Dictionary<string, string?>
            {
                [AuthenticationTicketStore.SessionId] = "session-abc"
            };

            if (enableVerifyProperty.HasValue)
            {
                items["enableVerify"] =
                    enableVerifyProperty.Value.ToString().ToLowerInvariant();
            }

            var authProperties = new AuthenticationProperties(items);

            return new CookieValidatePrincipalContext(
                new DefaultHttpContext(),
                new AuthenticationScheme(
                    "cookie",
                    null,
                    typeof(CookieAuthenticationHandler)),
                new CookieAuthenticationOptions(),
                new AuthenticationTicket(
                    principal,
                    authProperties,
                    "cookie"));
        }
    }
}