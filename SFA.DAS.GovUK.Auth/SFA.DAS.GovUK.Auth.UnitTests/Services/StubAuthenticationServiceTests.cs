using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.GovUK.Auth.Controllers;
using SFA.DAS.GovUK.Auth.Exceptions;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.UnitTests.Services;

[TestFixture]
public class StubAuthenticationServiceTests
{
    private StubAuthenticationService _sut;
    private Mock<ICustomClaims> _customClaimsMock;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private IConfiguration _configuration;

    [SetUp]
    public void SetUp()
    {
        _customClaimsMock = new Mock<ICustomClaims>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ResourceEnvironmentName", "dev" }
            }).Build();

        _sut = new StubAuthenticationService(_configuration, _customClaimsMock.Object, _httpContextAccessorMock.Object);
    }

    [Test]
    public async Task GetStubSignInClaims_ReturnsPrincipal_WithEmailAndSub()
    {
        // Arrange
        var details = new StubAuthUserDetails
        {
            Email = "test@example.com",
            Id = "abc-123",
            Mobile = "07123456789"
        };

        _customClaimsMock.Setup(x => x.GetClaims(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(new List<Claim> { new("custom", "value") });

        // Act
        var result = await _sut.GetStubSignInClaims(details);

        // Assert
        result.Identity!.Name.Should().BeNull(); // not set
        result.FindFirst(ClaimTypes.Email)?.Value.Should().Be("test@example.com");
        result.FindFirst(ClaimTypes.MobilePhone)?.Value.Should().Be("07123456789");
        result.FindFirst("sub")?.Value.Should().Be("abc-123");
        result.FindFirst("custom")?.Value.Should().Be("value");
    }

    [Test]
    public async Task GetStubSignInClaims_ReturnsNull_IfEnvironmentIsPRD()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ResourceEnvironmentName", "PRD" }
            }).Build();

        var sut = new StubAuthenticationService(config, _customClaimsMock.Object, _httpContextAccessorMock.Object);

        // Act
        var result = await sut.GetStubSignInClaims(new StubAuthUserDetails());

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetStubVerifyGovUkUser_ReturnsParsedUser_WhenJsonValid()
    {
        // Arrange
        var govUkUser = CreateGovUkUser();
        var json = JsonSerializer.Serialize(govUkUser);
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.Length).Returns(stream.Length);

        // Act
        var result = await _sut.GetStubVerifyGovUkUser(fileMock.Object);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public async Task GetStubVerifyGovUkUser_Throws_WhenInvalidJson()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("not-json"));
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.Length).Returns(stream.Length);

        // Act
        var act = async () => await _sut.GetStubVerifyGovUkUser(fileMock.Object);

        // Assert
        await act.Should().ThrowAsync<StubVerifyException>()
            .WithMessage("Invalid JSON file.");
    }

    [Test]
    public async Task GetAccountDetails_MapsClaimsFromHttpContext()
    {
        // Arrange
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("sub", "user-id"),
            new Claim(ClaimTypes.Email, "user@email.com"),
            new Claim(ClaimTypes.MobilePhone, "07123")
        });

        var claimsPrincipal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = claimsPrincipal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        // Act
        var result = await _sut.GetAccountDetails("any-token");

        // Assert
        result.Sub.Should().Be("user-id");
        result.Email.Should().Be("user@email.com");
        result.PhoneNumber.Should().Be("07123");
    }

    [Test]
    public async Task ChallengeWithVerifyAsync_SignsInAndRedirects()
    {
        // Arrange
        var context = new DefaultHttpContext();

        var identity = new ClaimsIdentity(new[] { new Claim("foo", "bar") }, "stub");
        context.User = new ClaimsPrincipal(identity);

        var authServiceMock = new Mock<IAuthenticationService>();
        var services = new ServiceCollection();
        services.AddSingleton(authServiceMock.Object);
        context.RequestServices = services.BuildServiceProvider();

        var controller = new VerifyIdentityController(Mock.Of<IGovUkAuthenticationService>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = context
            }
        };

        // Act
        var result = await _sut.ChallengeWithVerifyAsync("/return-here", controller);

        // Assert
        result.Should().BeOfType<LocalRedirectResult>();
        ((LocalRedirectResult)result).Url.Should().Be("/return-here");
    }

    private GovUkUser CreateGovUkUser()
    {
        var govUkUser = new GovUkUser
        {
            CoreIdentityJwt = new GovUkCoreIdentityJwt
            {
                Vc = new GovUkCoreIdentityCredential
                {
                    CredentialSubject = new GovUkCredentialSubject
                    {
                        Names = new List<GovUkName>
                {
                    new GovUkName
                    {
                        ValidFromRaw = "2020-03-01",
                        NameParts = new List<GovUkNamePart>
                        {
                            new GovUkNamePart
                            {
                                Value = "Alice",
                                Type = "GivenName"
                            },
                            new GovUkNamePart
                            {
                                Value = "Bobbin",
                                Type = "FamilyName"
                            }
                        }
                    }
                },
                        BirthDates = new List<GovUkBirthDateEntry>
                {
                    new GovUkBirthDateEntry
                    {
                        Value = "1970-01-01"
                    }
                }
                    }
                }
            }
        };

        return govUkUser;
    }
}
