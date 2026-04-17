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
        public async Task ValidatePrincipal_Updates_Claims_When_Verify_Enabled_By_Properties()
        {
            // Arrange
            _config.EnableVerify = null;

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                UserInfoClaims.CoreIdentityJWT.GetDescription(),
                CreateCoreIdentityJwtClaim("Jane", "Smith")));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));

            var validateContext = CreateContext(claimsIdentity, true);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm.P2");
            claimsIdentity.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Jane Smith");
            claimsIdentity.FindFirst(ClaimTypes.GivenName)!.Value.Should().Be("Jane");
            claimsIdentity.FindFirst(ClaimTypes.Surname)!.Value.Should().Be("Smith");

            validateContext.ShouldRenew.Should().BeTrue();
            _ticketStoreMock.Verify(x => x.RenewAsync("session-abc", It.IsAny<AuthenticationTicket>()), Times.Once);
        }

        [Test]
        public async Task ValidatePrincipal_Updates_Claims_When_Verify_Enabled_By_Config()
        {
            // Arrange
            _config.EnableVerify = "true";

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                UserInfoClaims.CoreIdentityJWT.GetDescription(),
                CreateCoreIdentityJwtClaim("Jane", "Smith")));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm.P2");
            claimsIdentity.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Jane Smith");
            claimsIdentity.FindFirst(ClaimTypes.GivenName)!.Value.Should().Be("Jane");
            claimsIdentity.FindFirst(ClaimTypes.Surname)!.Value.Should().Be("Smith");

            validateContext.ShouldRenew.Should().BeTrue();
            _ticketStoreMock.Verify(x => x.RenewAsync("session-abc", It.IsAny<AuthenticationTicket>()), Times.Once);
        }

        [Test]
        public async Task ValidatePrincipal_DoesNothing_When_Verify_Not_Enabled_By_Config_Or_Properties()
        {
            // Arrange
            _config.EnableVerify = null;

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                UserInfoClaims.CoreIdentityJWT.GetDescription(),
                CreateCoreIdentityJwtClaim("Jane", "Smith")));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));

            var validateContext = CreateContext(claimsIdentity, false);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm");
            claimsIdentity.FindFirst(ClaimTypes.Name).Should().BeNull();
            claimsIdentity.FindFirst(ClaimTypes.GivenName).Should().BeNull();
            claimsIdentity.FindFirst(ClaimTypes.Surname).Should().BeNull();

            validateContext.ShouldRenew.Should().BeFalse();
            _ticketStoreMock.Verify(x => x.RenewAsync(It.IsAny<string>(), It.IsAny<AuthenticationTicket>()), Times.Never);
        }

        [Test]
        public async Task ValidatePrincipal_DoesNothing_When_NoCoreIdentityJwt()
        {
            // Arrange
            _config.EnableVerify = "true";

            var claimsIdentity = new ClaimsIdentity();
            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot").Should().BeNull();
            claimsIdentity.FindFirst(ClaimTypes.Name).Should().BeNull();
            claimsIdentity.FindFirst(ClaimTypes.GivenName).Should().BeNull();
            claimsIdentity.FindFirst(ClaimTypes.Surname).Should().BeNull();

            validateContext.ShouldRenew.Should().BeFalse();
            _ticketStoreMock.Verify(x => x.RenewAsync(It.IsAny<string>(), It.IsAny<AuthenticationTicket>()), Times.Never);
        }

        [Test]
        public async Task ValidatePrincipal_DoesNothing_When_All_Claims_Already_Correct()
        {
            // Arrange
            _config.EnableVerify = "true";

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                UserInfoClaims.CoreIdentityJWT.GetDescription(),
                CreateCoreIdentityJwtClaim("Jane", "Smith")));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm.P2"));
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

            validateContext.ShouldRenew.Should().BeFalse();
            _ticketStoreMock.Verify(x => x.RenewAsync(It.IsAny<string>(), It.IsAny<AuthenticationTicket>()), Times.Never);
        }

        [Test]
        public async Task ValidatePrincipal_Updates_Name_Claims_When_Vot_Already_Correct_But_Names_Are_Missing()
        {
            // Arrange
            _config.EnableVerify = "true";

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                UserInfoClaims.CoreIdentityJWT.GetDescription(),
                CreateCoreIdentityJwtClaim("Jane", "Smith")));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm.P2"));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst("vot")!.Value.Should().Be("Cl.Cm.P2");
            claimsIdentity.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Jane Smith");
            claimsIdentity.FindFirst(ClaimTypes.GivenName)!.Value.Should().Be("Jane");
            claimsIdentity.FindFirst(ClaimTypes.Surname)!.Value.Should().Be("Smith");

            validateContext.ShouldRenew.Should().BeTrue();
            _ticketStoreMock.Verify(x => x.RenewAsync("session-abc", It.IsAny<AuthenticationTicket>()), Times.Once);
        }

        [Test]
        public async Task ValidatePrincipal_Uses_Latest_Currently_Valid_Historical_Name()
        {
            // Arrange
            _config.EnableVerify = "true";

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                UserInfoClaims.CoreIdentityJWT.GetDescription(),
                CreateCoreIdentityJwtClaim(
                    CreateHistoricalName("Old", "Name", "2020-01-01T00:00:00Z", "2022-01-01T00:00:00Z"),
                    CreateHistoricalName("Current", "Name", "2022-01-01T00:00:00Z", null))));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Current Name");
            claimsIdentity.FindFirst(ClaimTypes.GivenName)!.Value.Should().Be("Current");
            claimsIdentity.FindFirst(ClaimTypes.Surname)!.Value.Should().Be("Name");

            validateContext.ShouldRenew.Should().BeTrue();
            _ticketStoreMock.Verify(x => x.RenewAsync("session-abc", It.IsAny<AuthenticationTicket>()), Times.Once);
        }

        [Test]
        public async Task ValidatePrincipal_Falls_Back_To_Latest_Overall_Name_When_No_Name_Is_Currently_Valid()
        {
            // Arrange
            _config.EnableVerify = "true";

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                UserInfoClaims.CoreIdentityJWT.GetDescription(),
                CreateCoreIdentityJwtClaim(
                    CreateHistoricalName("Older", "Name", "2018-01-01T00:00:00Z", "2019-01-01T00:00:00Z"),
                    CreateHistoricalName("Latest", "Expired", "2020-01-01T00:00:00Z", "2021-01-01T00:00:00Z"))));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));

            var validateContext = CreateContext(claimsIdentity, null);

            // Act
            await _sut.ValidatePrincipal(validateContext);

            // Assert
            claimsIdentity.FindFirst(ClaimTypes.Name)!.Value.Should().Be("Latest Expired");
            claimsIdentity.FindFirst(ClaimTypes.GivenName)!.Value.Should().Be("Latest");
            claimsIdentity.FindFirst(ClaimTypes.Surname)!.Value.Should().Be("Expired");
        }

        [Test]
        public async Task ValidatePrincipal_Sets_Only_GivenName_And_FullName_When_FamilyName_Is_Missing()
        {
            // Arrange
            _config.EnableVerify = "true";

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                UserInfoClaims.CoreIdentityJWT.GetDescription(),
                CreateCoreIdentityJwtClaim(
                    CreateHistoricalName("Cher", null, "2020-01-01T00:00:00Z", null))));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));

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

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(
                UserInfoClaims.CoreIdentityJWT.GetDescription(),
                CreateCoreIdentityJwtClaim(
                    CreateHistoricalName(null, "Madonna", "2020-01-01T00:00:00Z", null))));
            claimsIdentity.AddClaim(new Claim("vot", "Cl.Cm"));

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

            var items = new Dictionary<string, string?>
            {
                [AuthenticationTicketStore.SessionId] = "session-abc"
            };

            if (enableVerifyProperty.HasValue)
            {
                items["enableVerify"] = enableVerifyProperty.Value.ToString().ToLowerInvariant();
            }

            var authProperties = new AuthenticationProperties(items);

            var context = new DefaultHttpContext();

            return new CookieValidatePrincipalContext(
                context,
                new AuthenticationScheme("cookie", null, typeof(CookieAuthenticationHandler)),
                new CookieAuthenticationOptions(),
                new AuthenticationTicket(principal, authProperties, "cookie"));
        }

        private static string CreateCoreIdentityJwtClaim(string givenName, string familyName)
        {
            return CreateCoreIdentityJwtClaim(CreateHistoricalName(givenName, familyName, "2020-01-01T00:00:00Z", null));
        }

        private static string CreateCoreIdentityJwtClaim(params GovUkName[] names)
        {
            var payload = new GovUkCoreIdentityJwt
            {
                Sub = "subject-123",
                Vot = "Cl.Cm.P2",
                Vc = new GovUkCoreIdentityCredential
                {
                    CredentialSubject = new GovUkCredentialSubject
                    {
                        Names = names.ToList()
                    }
                }
            };

            return CoreIdentityJwtConverter.SerializeStubCoreIdentityJwt(payload);
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