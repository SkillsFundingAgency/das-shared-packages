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
        private const string SuspendedUrl = "/suspended";
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
                SuspendedUrl,
                LoginRedirect);
        }

        [Test]
        public async Task SigningOut_DeletesCookie_AndRedirects()
        {
            // Arrange
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

            // Act
            await _sut.SigningOut(signingOutContext);

            // Assert
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
        public async Task ValidatePrincipal_Updates_Claims_When_Verify_Enabled_By_Properties()
        {
            // Arrange
            _config.EnableVerify = null;
            _config.RequestedUserInfoClaims = UserInfoClaims.CoreIdentityJWT.ToString();

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));
            claimsIdentity.AddClaim(new Claim(
                StubAuthenticationService.StubGovUkUserClaimType,
                CreateStubGovUkUserJson("Jane", "Smith")));

            var validateContext = CreateContext(claimsIdentity, true);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm.P2");
            claimsIdentity.FindFirst(UserInfoClaims.CoreIdentityJWT.GetDescription())!.Value.Should().NotBeNullOrWhiteSpace();
            claimsIdentity.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Jane Smith");
            claimsIdentity.FindFirst(ClaimTypes.GivenName)!.Value.Should().Be("Jane");
            claimsIdentity.FindFirst(ClaimTypes.Surname)!.Value.Should().Be("Smith");

            validateContext.ShouldRenew.Should().BeTrue();
            _ticketStoreMock.Verify(x => x.RenewAsync("session-123", It.IsAny<AuthenticationTicket>()), Times.Once);
        }

        [Test]
        public async Task ValidatePrincipal_Updates_Claims_When_Verify_Enabled_By_Config()
        {
            // Arrange
            _config.EnableVerify = "true";
            _config.RequestedUserInfoClaims = UserInfoClaims.CoreIdentityJWT.ToString();

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));
            claimsIdentity.AddClaim(new Claim(
                StubAuthenticationService.StubGovUkUserClaimType,
                CreateStubGovUkUserJson("Jane", "Smith")));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm.P2");
            claimsIdentity.FindFirst(UserInfoClaims.CoreIdentityJWT.GetDescription())!.Value.Should().NotBeNullOrWhiteSpace();
            claimsIdentity.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Jane Smith");
            claimsIdentity.FindFirst(ClaimTypes.GivenName)!.Value.Should().Be("Jane");
            claimsIdentity.FindFirst(ClaimTypes.Surname)!.Value.Should().Be("Smith");

            validateContext.ShouldRenew.Should().BeTrue();
            _ticketStoreMock.Verify(x => x.RenewAsync("session-123", It.IsAny<AuthenticationTicket>()), Times.Once);
        }

        [Test]
        public async Task ValidatePrincipal_Sets_ClCm_When_Verify_Not_Enabled()
        {
            // Arrange
            _config.EnableVerify = null;

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                StubAuthenticationService.StubGovUkUserClaimType,
                CreateStubGovUkUserJson("Jane", "Smith")));

            var validateContext = CreateContext(claimsIdentity, false);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm");
            claimsIdentity.FindFirst(UserInfoClaims.CoreIdentityJWT.GetDescription()).Should().BeNull();
            claimsIdentity.FindFirst(ClaimTypes.Name).Should().BeNull();
            claimsIdentity.FindFirst(ClaimTypes.GivenName).Should().BeNull();
            claimsIdentity.FindFirst(ClaimTypes.Surname).Should().BeNull();

            validateContext.ShouldRenew.Should().BeTrue();
            _ticketStoreMock.Verify(x => x.RenewAsync("session-123", It.IsAny<AuthenticationTicket>()), Times.Once);
        }

        [Test]
        public async Task ValidatePrincipal_DoesNothing_When_Verify_Not_Enabled_And_Vot_Already_ClCm()
        {
            // Arrange
            _config.EnableVerify = null;

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));
            claimsIdentity.AddClaim(new Claim(
                StubAuthenticationService.StubGovUkUserClaimType,
                CreateStubGovUkUserJson("Jane", "Smith")));

            var validateContext = CreateContext(claimsIdentity, false);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm");
            claimsIdentity.FindFirst(UserInfoClaims.CoreIdentityJWT.GetDescription()).Should().BeNull();
            claimsIdentity.FindFirst(ClaimTypes.Name).Should().BeNull();

            validateContext.ShouldRenew.Should().BeFalse();
            _ticketStoreMock.Verify(x => x.RenewAsync(It.IsAny<string>(), It.IsAny<AuthenticationTicket>()), Times.Never);
        }

        [Test]
        public async Task ValidatePrincipal_Adds_UserInfoClaim_When_VerifyEnabled()
        {
            // Arrange
            _config.EnableVerify = null;
            _config.RequestedUserInfoClaims = UserInfoClaims.Address.ToString();

            var user = new GovUkUser
            {
                Addresses = new List<GovUkAddress>
                {
                    new GovUkAddress
                    {
                        BuildingNumber = "1",
                        StreetName = "Test Lane"
                    }
                }
            };

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(StubAuthenticationService.StubGovUkUserClaimType, JsonSerializer.Serialize(user)));

            var validateContext = CreateContext(claimsIdentity, true);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            var claim = claimsIdentity.FindFirst(UserInfoClaims.Address.GetDescription());
            claim.Should().NotBeNull();

            validateContext.ShouldRenew.Should().BeTrue();
            _ticketStoreMock.Verify(x => x.RenewAsync("session-123", It.IsAny<AuthenticationTicket>()), Times.Once);
        }

        [Test]
        public async Task ValidatePrincipal_Adds_UserInfoClaims_When_Vot_Already_Correct()
        {
            // Arrange
            _config.EnableVerify = "true";
            _config.RequestedUserInfoClaims = UserInfoClaims.Address.ToString();

            var user = new GovUkUser
            {
                Addresses = new List<GovUkAddress>
                {
                    new GovUkAddress
                    {
                        BuildingNumber = "1",
                        StreetName = "Test Lane"
                    }
                }
            };

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm.P2"));
            claimsIdentity.AddClaim(new Claim(StubAuthenticationService.StubGovUkUserClaimType, JsonSerializer.Serialize(user)));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst(UserInfoClaims.Address.GetDescription()).Should().NotBeNull();

            validateContext.ShouldRenew.Should().BeTrue();
            _ticketStoreMock.Verify(x => x.RenewAsync("session-123", It.IsAny<AuthenticationTicket>()), Times.Once);
        }

        [Test]
        public async Task ValidatePrincipal_Adds_Name_Claims_From_CoreIdentityJwt()
        {
            // Arrange
            _config.EnableVerify = "true";
            _config.RequestedUserInfoClaims = UserInfoClaims.CoreIdentityJWT.ToString();

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));
            claimsIdentity.AddClaim(new Claim(
                StubAuthenticationService.StubGovUkUserClaimType,
                CreateStubGovUkUserJson("Jane", "Smith")));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Jane Smith");
            claimsIdentity.FindFirst(ClaimTypes.GivenName)!.Value.Should().Be("Jane");
            claimsIdentity.FindFirst(ClaimTypes.Surname)!.Value.Should().Be("Smith");
        }

        [Test]
        public async Task ValidatePrincipal_Uses_Latest_Currently_Valid_Historical_Name()
        {
            // Arrange
            _config.EnableVerify = "true";
            _config.RequestedUserInfoClaims = UserInfoClaims.CoreIdentityJWT.ToString();

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));
            claimsIdentity.AddClaim(new Claim(
                StubAuthenticationService.StubGovUkUserClaimType,
                CreateStubGovUkUserJson(
                    CreateHistoricalName("Old", "Name", "2020-01-01T00:00:00Z", "2022-01-01T00:00:00Z"),
                    CreateHistoricalName("Current", "Name", "2022-01-01T00:00:00Z", null))));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Current Name");
            claimsIdentity.FindFirst(ClaimTypes.GivenName)!.Value.Should().Be("Current");
            claimsIdentity.FindFirst(ClaimTypes.Surname)!.Value.Should().Be("Name");
        }

        [Test]
        public async Task ValidatePrincipal_Sets_Only_GivenName_And_FullName_When_FamilyName_Is_Missing()
        {
            // Arrange
            _config.EnableVerify = "true";
            _config.RequestedUserInfoClaims = UserInfoClaims.CoreIdentityJWT.ToString();

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));
            claimsIdentity.AddClaim(new Claim(
                StubAuthenticationService.StubGovUkUserClaimType,
                CreateStubGovUkUserJson(
                    CreateHistoricalName("Cher", null, "2020-01-01T00:00:00Z", null))));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Cher");
            claimsIdentity.FindFirst(ClaimTypes.GivenName)!.Value.Should().Be("Cher");
            claimsIdentity.FindFirst(ClaimTypes.Surname).Should().BeNull();
        }

        [Test]
        public async Task ValidatePrincipal_Sets_Only_Surname_And_FullName_When_GivenName_Is_Missing()
        {
            // Arrange
            _config.EnableVerify = "true";
            _config.RequestedUserInfoClaims = UserInfoClaims.CoreIdentityJWT.ToString();

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));
            claimsIdentity.AddClaim(new Claim(
                StubAuthenticationService.StubGovUkUserClaimType,
                CreateStubGovUkUserJson(
                    CreateHistoricalName(null, "Madonna", "2020-01-01T00:00:00Z", null))));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Madonna");
            claimsIdentity.FindFirst(ClaimTypes.GivenName).Should().BeNull();
            claimsIdentity.FindFirst(ClaimTypes.Surname)!.Value.Should().Be("Madonna");
        }

        private CookieValidatePrincipalContext CreateContext(ClaimsIdentity claimsIdentity, bool? enableVerifyProperty)
        {
            var principal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                [AuthenticationTicketStore.SessionId] = "session-123"
            });

            if (enableVerifyProperty.HasValue)
            {
                authProperties.Items["enableVerify"] = enableVerifyProperty.Value.ToString().ToLowerInvariant();
            }

            var context = new DefaultHttpContext();

            return new CookieValidatePrincipalContext(
                context,
                new AuthenticationScheme("cookie", null, typeof(CookieAuthenticationHandler)),
                new CookieAuthenticationOptions(),
                new AuthenticationTicket(principal, authProperties, "cookie"));
        }

        private static string CreateStubGovUkUserJson(string givenName, string familyName)
        {
            return CreateStubGovUkUserJson(CreateHistoricalName(givenName, familyName, "2020-01-01T00:00:00Z", null));
        }

        private static string CreateStubGovUkUserJson(params GovUkName[] names)
        {
            var user = new GovUkUser
            {
                CoreIdentityJwt = new GovUkCoreIdentityJwt
                {
                    Sub = "sub-xyz",
                    Vot = "Cl.Cm.P2",
                    Vc = new GovUkCoreIdentityCredential
                    {
                        CredentialSubject = new GovUkCredentialSubject
                        {
                            Names = names.ToList()
                        }
                    }
                }
            };

            return JsonSerializer.Serialize(user);
        }

        private static GovUkName CreateHistoricalName(
            string givenName,
            string familyName,
            string validFrom,
            string validUntil)
        {
            var nameParts = new List<GovUkNamePart>();

            if (givenName != null)
            {
                nameParts.Add(new GovUkNamePart
                {
                    Type = "GivenName",
                    Value = givenName,
                    ValidFromRaw = validFrom,
                    ValidUntilRaw = validUntil
                });
            }

            if (familyName != null)
            {
                nameParts.Add(new GovUkNamePart
                {
                    Type = "FamilyName",
                    Value = familyName,
                    ValidFromRaw = validFrom,
                    ValidUntilRaw = validUntil
                });
            }

            return new GovUkName
            {
                ValidFromRaw = validFrom,
                ValidUntilRaw = validUntil,
                NameParts = nameParts
            };
        }
    }
}