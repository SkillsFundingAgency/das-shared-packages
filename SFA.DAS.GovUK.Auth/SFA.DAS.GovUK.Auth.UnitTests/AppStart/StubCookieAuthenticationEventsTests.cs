using System.Security.Claims;
using System.Text.Json;
using System.Web;
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
    public class StubCookieAuthenticationEventsTests
    {
        private Mock<IOptions<GovUkOidcConfiguration>> _configMock;
        private Mock<ITicketStore> _ticketStoreMock;
        private StubCookieAuthenticationEvents _sut;
        private GovUkOidcConfiguration _config;

        private const string SignedOutUrl = "/signed-out";
        private const string LoginRedirect = "/custom-login";

        [SetUp]
        public void SetUp()
        {
            _config = new GovUkOidcConfiguration
            {
                RequestedUserInfoClaims = "CoreIdentityJWT,Address",
                LoginSlidingExpiryTimeOutInMinutes = 60
            };

            _configMock = new Mock<IOptions<GovUkOidcConfiguration>>();
            _configMock.Setup(x => x.Value).Returns(_config);

            _ticketStoreMock = new Mock<ITicketStore>();

            _sut = new StubCookieAuthenticationEvents(
                _configMock.Object,
                _ticketStoreMock.Object,
                SignedOutUrl,
                LoginRedirect);
        }

        [Test]
        public async Task SigningOut_DeletesCookie_AndRedirects()
        {
            var responseMock = new Mock<HttpResponse>();
            var cookiesMock = new Mock<IResponseCookies>();
            var headers = new HeaderDictionary();

            responseMock.Setup(r => r.Cookies).Returns(cookiesMock.Object);
            responseMock.Setup(r => r.Redirect(It.IsAny<string>()))
                        .Callback<string>(url => headers["Location"] = url);

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(c => c.Response).Returns(responseMock.Object);

            var signingOutContext = new CookieSigningOutContext(
                contextMock.Object,
                new AuthenticationScheme("cookie", null, typeof(CookieAuthenticationHandler)),
                new CookieAuthenticationOptions(),
                new AuthenticationProperties(),
                new CookieOptions());

            await _sut.SigningOut(signingOutContext);

            headers["Location"].ToString().Should().Be(SignedOutUrl);
            cookiesMock.Verify(c => c.Delete(GovUkConstants.StubAuthCookieName), Times.Once);
        }

        [Test]
        public async Task RedirectToLogin_Redirects_To_Expected_Encoded_ReturnUrl()
        {
            // Arrange
            var requestUrl = "https://example.com/Account/Login?ReturnUrl=%2Fsecure%2Farea";
            var expectedEncodedReturnUrl = HttpUtility.UrlEncode("https://example.com/secure/area");
            var expectedRedirectUrl = $"{LoginRedirect}?ReturnUrl={expectedEncodedReturnUrl}";

            var responseMock = new Mock<HttpResponse>();
            responseMock.Setup(r => r.Redirect(It.IsAny<string>()));

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(c => c.Response).Returns(responseMock.Object);

            var redirectContext = new RedirectContext<CookieAuthenticationOptions>(
                contextMock.Object,
                new AuthenticationScheme("cookie", null, typeof(CookieAuthenticationHandler)),
                new CookieAuthenticationOptions(),
                new AuthenticationProperties(),
                requestUrl);

            // Act
            await _sut.RedirectToLogin(redirectContext);

            // Assert
            responseMock.Verify(r => r.Redirect(expectedRedirectUrl), Times.Once);
        }

        [Test]
        public async Task ValidatePrincipal_Adds_ClCmP2_And_Claims_And_RenewsTicket()
        {
            // Arrange
            var claimsIdentity = new ClaimsIdentity();
            var govUkUser = new GovUkUser { CoreIdentityJwt = new GovUkCoreIdentityJwt() };
            var govUkUserJson = JsonSerializer.Serialize(govUkUser);

            claimsIdentity.AddClaim(new Claim(StubAuthenticationService.StubGovUkUserClaimType, govUkUserJson));
            var principal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                ["enableVerify"] = "true",
                [AuthenticationTicketStore.SessionId] = "session-123"
            });

            var ticket = new AuthenticationTicket(principal, authProperties, "cookie");

            var context = new DefaultHttpContext();

            var validateContext = new CookieValidatePrincipalContext(
                context,
                new AuthenticationScheme("cookie", null, typeof(CookieAuthenticationHandler)),
                new CookieAuthenticationOptions(),
                ticket);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            var votClaim = claimsIdentity.FindFirst("vot");
            votClaim.Should().NotBeNull();
            votClaim!.Value.Should().Be("Cl.Cm.P2");

            _ticketStoreMock.Verify(x => x.RenewAsync("session-123", It.IsAny<AuthenticationTicket>()), Times.Once);
        }

        [TestCase(UserInfoClaims.CoreIdentityJWT)]
        [TestCase(UserInfoClaims.Address)]
        [TestCase(UserInfoClaims.Passport)]
        [TestCase(UserInfoClaims.DrivingPermit)]
        public async Task ValidatePrincipal_Adds_UserInfoClaim_When_VerifyEnabled(UserInfoClaims claimType)
        {
            // Arrange
            var user = new GovUkUser
            {
                CoreIdentityJwt = new GovUkCoreIdentityJwt { Sub = "sub-xyz" },
                Addresses = new List<GovUkAddress> { new GovUkAddress { BuildingNumber = "1", StreetName = "Test Lane" } },
                Passports = new List<GovUkPassport> { new GovUkPassport { DocumentNumber = "ABC123" } },
                DrivingPermits = new List<GovUkDrivingPermit> { new GovUkDrivingPermit { IssueNumber = "LIC456" } }
            };

            _config.RequestedUserInfoClaims = claimType.ToString();

            var userJson = JsonSerializer.Serialize(user);

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(StubAuthenticationService.StubGovUkUserClaimType, userJson));
            var principal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                ["enableVerify"] = "true",
                [AuthenticationTicketStore.SessionId] = "session-test"
            });

            var ticket = new AuthenticationTicket(principal, authProperties, "cookie");
            var context = new DefaultHttpContext();

            var validateContext = new CookieValidatePrincipalContext(
                context,
                new AuthenticationScheme("cookie", null, typeof(CookieAuthenticationHandler)),
                new CookieAuthenticationOptions(),
                ticket);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            var claim = claimsIdentity.FindFirst(claimType.GetDescription());
            claim.Should().NotBeNull($"Claim '{claimType}' should be added when requested and verify is enabled.");

            _ticketStoreMock.Verify(x => x.RenewAsync("session-test", It.IsAny<AuthenticationTicket>()), Times.Once);
        }
    }
}
