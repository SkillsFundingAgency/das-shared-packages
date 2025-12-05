using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Extensions;
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

            _sut = new GovUkCookieAuthenticationEvents(_configMock.Object, _ticketStoreMock.Object);
        }

        [Test]
        public async Task ValidatePrincipal_UpdatesVotClaim_WhenCoreIdentityJwtPresentAndNoP2()
        {
            // Arrange
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(UserInfoClaims.CoreIdentityJWT.GetDescription(), "fake-jwt"));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));

            var principal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                ["enableVerify"] = "true",
                [AuthenticationTicketStore.SessionId] = "session-abc"
            });

            var context = new DefaultHttpContext();
            var validateContext = new CookieValidatePrincipalContext(
                context,
                new AuthenticationScheme("cookie", null, typeof(CookieAuthenticationHandler)),
                new CookieAuthenticationOptions(),
                new AuthenticationTicket(principal, authProperties, "cookie"));

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            var votClaim = claimsIdentity.FindFirst("vot");
            votClaim.Should().NotBeNull();
            votClaim!.Value.Should().Be("Cl.Cm.P2");

            _ticketStoreMock.Verify(x => x.RenewAsync("session-abc", It.IsAny<AuthenticationTicket>()), Times.Once);
            validateContext.ShouldRenew.Should().BeTrue();
        }

        [Test]
        public async Task ValidatePrincipal_DoesNothing_WhenNoCoreIdentityJwt()
        {
            // Arrange
            var claimsIdentity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                ["enableVerify"] = "true",
                [AuthenticationTicketStore.SessionId] = "session-abc"
            });

            var context = new DefaultHttpContext();
            var validateContext = new CookieValidatePrincipalContext(
                context,
                new AuthenticationScheme("cookie", null, typeof(CookieAuthenticationHandler)),
                new CookieAuthenticationOptions(),
                new AuthenticationTicket(principal, authProperties, "cookie"));

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.HasClaim(c => c.Type == "vot").Should().BeFalse();
            _ticketStoreMock.Verify(x => x.RenewAsync(It.IsAny<string>(), It.IsAny<AuthenticationTicket>()), Times.Never);
            validateContext.ShouldRenew.Should().BeFalse();
        }

        [Test]
        public async Task ValidatePrincipal_DoesNothing_WhenVotAlreadyContainsP2()
        {
            // Arrange
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(UserInfoClaims.CoreIdentityJWT.GetDescription(), "fake-jwt"));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm.P2"));

            var principal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                ["enableVerify"] = "true",
                [AuthenticationTicketStore.SessionId] = "session-abc"
            });

            var context = new DefaultHttpContext();
            var validateContext = new CookieValidatePrincipalContext(
                context,
                new AuthenticationScheme("cookie", null, typeof(CookieAuthenticationHandler)),
                new CookieAuthenticationOptions(),
                new AuthenticationTicket(principal, authProperties, "cookie"));

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            var votClaim = claimsIdentity.FindFirst("vot");
            votClaim.Should().NotBeNull();
            votClaim!.Value.Should().Be("Cl.Cm.P2");

            _ticketStoreMock.Verify(x => x.RenewAsync(It.IsAny<string>(), It.IsAny<AuthenticationTicket>()), Times.Never);
            validateContext.ShouldRenew.Should().BeFalse();
        }
    }
}
