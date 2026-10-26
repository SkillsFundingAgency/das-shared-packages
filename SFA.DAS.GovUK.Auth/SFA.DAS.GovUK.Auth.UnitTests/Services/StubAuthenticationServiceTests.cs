using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Controllers;
using SFA.DAS.GovUK.Auth.Exceptions;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.UnitTests.Services;

[TestFixture]
public class StubAuthenticationServiceTests
{
    private StubAuthenticationService _sut;
    private GovUkOidcConfiguration _config;
    private Mock<IOptions<GovUkOidcConfiguration>> _configMock;
    private Mock<ICustomClaims> _customClaimsMock;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private IConfiguration _configuration;

    [SetUp]
    public void SetUp()
    {
        _customClaimsMock = new Mock<ICustomClaims>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext());

        _config = new GovUkOidcConfiguration
        {
            RequestedUserInfoClaims = "CoreIdentityJWT,Address"
        };

        _configMock = new Mock<IOptions<GovUkOidcConfiguration>>();
        _configMock.Setup(x => x.Value).Returns(_config);

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ResourceEnvironmentName", "DEV" }
            }).Build();

        _sut = new StubAuthenticationService(
            _configuration,
            _configMock.Object,
            _customClaimsMock.Object,
            _httpContextAccessorMock.Object);
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

        _customClaimsMock
            .Setup(x => x.GetClaims(It.IsAny<TokenValidatedContext>()))
            .ReturnsAsync(new List<Claim> { new("custom", "value") });

        // Act
        var result = await _sut.GetStubSignInClaims(details);

        // Assert
        result.ResponseHandled.Should().BeFalse();
        result.Principal.Identity!.Name.Should().BeNull();
        result.Principal.FindFirst(ClaimTypes.Email)?.Value
            .Should().Be("test@example.com");
        result.Principal.FindFirst(ClaimTypes.MobilePhone)?.Value
            .Should().Be("07123456789");
        result.Principal.FindFirst("sub")?.Value.Should().Be("abc-123");
        result.Principal.FindFirst("custom")?.Value.Should().Be("value");

        var userInfo = JsonSerializer.Deserialize<GovUkUser>(
            result.Principal.FindFirst(GovUkUserClaimTypes.UserInfo)!.Value);

        userInfo!.Sub.Should().Be("abc-123");
        userInfo.Email.Should().Be("test@example.com");
        userInfo.PhoneNumber.Should().Be("07123456789");
    }

    [Test]
    public async Task GetStubSignInClaims_CombinesFormDetailsWithUploadedVerifyDetails()
    {
        // Arrange
        var uploadedUser = CreateGovUkUser();
        uploadedUser.Addresses = new List<GovUkAddress>
        {
            new GovUkAddress { StreetName = "Test Lane" }
        };

        var details = new StubAuthUserDetails
        {
            Id = "form-id",
            Email = "form@example.com",
            GovUkUser = uploadedUser
        };

        _customClaimsMock
            .Setup(x => x.GetClaims(It.IsAny<TokenValidatedContext>()))
            .ReturnsAsync(Array.Empty<Claim>());

        // Act
        var result = await _sut.GetStubSignInClaims(details);

        // Assert
        result.ResponseHandled.Should().BeFalse();
        result.Principal.HasClaim(
            GovUkUserClaimTypes.VerifiedIdentity, "true").Should().BeTrue();

        var userInfo = JsonSerializer.Deserialize<GovUkUser>(
            result.Principal.FindFirst(GovUkUserClaimTypes.UserInfo)!.Value);

        userInfo!.Sub.Should().Be("form-id");
        userInfo.Email.Should().Be("form@example.com");
        userInfo.CoreIdentityJwt.Should().NotBeNull();
        userInfo.Addresses.Single().StreetName.Should().Be("Test Lane");
    }

    [Test]
    public async Task GetStubSignInClaims_AddsCustomClaimsToReplacementPrincipal()
    {
        // Arrange
        var replacementPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[] { new Claim("replacement", "true") },
                CookieAuthenticationDefaults.AuthenticationScheme));

        _customClaimsMock
            .Setup(x => x.GetClaims(It.IsAny<TokenValidatedContext>()))
            .Returns((TokenValidatedContext context) =>
            {
                context.Principal = replacementPrincipal;

                return Task.FromResult<IEnumerable<Claim>>(
                    new[] { new Claim("custom", "value") });
            });

        var details = new StubAuthUserDetails
        {
            Id = "abc-123",
            Email = "test@example.com"
        };

        // Act
        var result = await _sut.GetStubSignInClaims(details);

        // Assert
        result.ResponseHandled.Should().BeFalse();
        result.Principal.Should().BeSameAs(replacementPrincipal);
        result.Principal.FindFirst("custom")?.Value.Should().Be("value");
    }

    [Test]
    public async Task GetStubSignInClaims_ReturnsHandled_WhenCustomClaimsHandlesResponse()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(httpContext);

        _customClaimsMock
            .Setup(x => x.GetClaims(It.IsAny<TokenValidatedContext>()))
            .Returns((TokenValidatedContext context) =>
            {
                context.Response.Redirect("/Home/AccessDenied");
                context.HandleResponse();

                return Task.FromResult<IEnumerable<Claim>>(
                    Array.Empty<Claim>());
            });

        var details = new StubAuthUserDetails
        {
            Id = "abc-123",
            Email = "test@example.com"
        };

        // Act
        var result = await _sut.GetStubSignInClaims(details);

        // Assert
        result.ResponseHandled.Should().BeTrue();
        result.Principal.Should().BeNull();
        httpContext.Response.Headers.Location.ToString()
            .Should().Be("/Home/AccessDenied");
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
        result.CoreIdentityJwt.Should().NotBeNull();
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
        var act = async () =>
            await _sut.GetStubVerifyGovUkUser(fileMock.Object);

        // Assert
        await act.Should().ThrowAsync<StubVerifyException>()
            .WithMessage("Invalid JSON file.");
    }

    [Test]
    public void GetAccountDetails_ReturnsUserInfoFromHttpContext()
    {
        // Arrange
        var storedUser = new GovUkUser
        {
            Sub = "user-id",
            Email = "user@email.com",
            PhoneNumber = "07123"
        };

        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(
                    GovUkUserClaimTypes.UserInfo,
                    JsonSerializer.Serialize(storedUser))
            },
            CookieAuthenticationDefaults.AuthenticationScheme);

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(context);

        // Act
        var result = _sut.GetAccountDetails();

        // Assert
        result.Should().NotBeNull();
        result.Sub.Should().Be("user-id");
        result.Email.Should().Be("user@email.com");
        result.PhoneNumber.Should().Be("07123");
    }

    [Test]
    public void GetAccountDetails_ReturnsNull_WhenPrincipalIsUnauthenticated()
    {
        // Arrange
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(
                    GovUkUserClaimTypes.UserInfo,
                    JsonSerializer.Serialize(CreateGovUkUser()))
            });

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            });

        // Act
        var result = _sut.GetAccountDetails();

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ChallengeWithVerifyAsync_SignsInAndRedirects()
    {
        // Arrange
        var context = new DefaultHttpContext();

        var identity = new ClaimsIdentity(
            new[] { new Claim("foo", "bar") },
            "stub");
        context.User = new ClaimsPrincipal(identity);

        var authServiceMock = new Mock<IAuthenticationService>();
        var services = new ServiceCollection();
        services.AddSingleton(authServiceMock.Object);
        context.RequestServices = services.BuildServiceProvider();

        var controller = new VerifyIdentityController(
            Mock.Of<IGovUkAuthenticationService>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = context
            }
        };

        // Act
        var result = await _sut.ChallengeWithVerifyAsync(
            "/return-here", controller);

        // Assert
        result.Should().BeOfType<LocalRedirectResult>();
        ((LocalRedirectResult)result).Url.Should().Be("/return-here");
    }

    private GovUkUser CreateGovUkUser()
    {
        return new GovUkUser
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
    }
}