using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SFA.DAS.GovUK.Auth.Extensions;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Validation;

namespace SFA.DAS.GovUK.Auth.UnitTests.Validation;

[TestFixture]
public class ValidateCoreIdentityJwtClaimActionTests
{
    private Mock<ICoreIdentityJwtValidator> _coreIdentityHelperMock;
    private ValidateCoreIdentityJwtClaimAction _sut;

    [SetUp]
    public void SetUp()
    {
        _coreIdentityHelperMock = new Mock<ICoreIdentityJwtValidator>();
        _sut = new ValidateCoreIdentityJwtClaimAction(_coreIdentityHelperMock.Object);
    }

    [Test]
    public void Run_DoesNothing_IfCoreIdentityClaimIsMissing()
    {
        // Arrange
        var json = JsonDocument.Parse("{}").RootElement;
        var identity = new ClaimsIdentity();

        // Act
        _sut.Run(json, identity, "issuer");

        // Assert
        identity.Claims.Should().BeEmpty();
        _coreIdentityHelperMock.Verify(
            h => h.ValidateCoreIdentity(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void Run_ThrowsException_IfSubClaimsDoNotMatch()
    {
        // Arrange
        const string token = "fake.jwt.token";
        var json = CreateUserInfo(token);
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "actual-sub")
        });

        var principalWithWrongSub = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("sub", "different-sub")
        }));

        _coreIdentityHelperMock
            .Setup(h => h.ValidateCoreIdentity(token))
            .Returns(principalWithWrongSub);

        // Act
        var act = () => _sut.Run(json, identity, "issuer");

        // Assert
        act.Should().Throw<SecurityTokenException>()
            .WithMessage("The 'sub' claim in the core identity JWT does not match the 'sub' claim from the ID token.");
        identity.HasClaim(GovUkUserClaimTypes.VerifiedIdentity, "true")
            .Should().BeFalse();
    }

    [Test]
    public void Run_AddsLatestNameAndVerifiedIdentity_WhenSubClaimsMatch()
    {
        // Arrange
        const string subject = "matching-sub";
        var token = CreateToken(subject,
            CreateName("Old", "Name", "2020-01-01", "2022-01-01"),
            CreateName("Jane", "Smith", "2022-01-01", null));

        var json = CreateUserInfo(token);
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, subject),
            new Claim(ClaimTypes.GivenName, "Previous"),
            new Claim(ClaimTypes.Surname, "Person"),
            new Claim(ClaimTypes.Name, "Previous Person")
        });

        _coreIdentityHelperMock
            .Setup(h => h.ValidateCoreIdentity(token))
            .Returns(CreateCoreIdentityPrincipal(subject));

        // Act
        _sut.Run(json, identity, "issuer");

        // Assert
        identity.FindAll(ClaimTypes.GivenName).Should()
            .ContainSingle().Which.Value.Should().Be("Jane");
        identity.FindAll(ClaimTypes.Surname).Should()
            .ContainSingle().Which.Value.Should().Be("Smith");
        identity.FindAll(ClaimTypes.Name).Should()
            .ContainSingle().Which.Value.Should().Be("Jane Smith");
        identity.FindAll(GovUkUserClaimTypes.VerifiedIdentity).Should()
            .ContainSingle().Which.Value.Should().Be("true");
    }

    [Test]
    public void Run_ThrowsException_IfCredentialSubjectIsMissing()
    {
        // Arrange
        const string subject = "matching-sub";
        var token = CoreIdentityJwtConverter.SerializeStubCoreIdentityJwt(
            new GovUkCoreIdentityJwt { Sub = subject });
        var json = CreateUserInfo(token);
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, subject)
        });

        _coreIdentityHelperMock
            .Setup(h => h.ValidateCoreIdentity(token))
            .Returns(CreateCoreIdentityPrincipal(subject));

        // Act
        var act = () => _sut.Run(json, identity, "issuer");

        // Assert
        act.Should().Throw<SecurityTokenException>()
            .WithMessage("The core identity contains no credential subject.");
        identity.HasClaim(GovUkUserClaimTypes.VerifiedIdentity, "true")
            .Should().BeFalse();
    }

    [Test]
    public void Run_ThrowsException_IfCredentialSubjectHasNoUsableName()
    {
        // Arrange
        const string subject = "matching-sub";
        var token = CreateToken(subject);
        var json = CreateUserInfo(token);
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, subject)
        });

        _coreIdentityHelperMock
            .Setup(h => h.ValidateCoreIdentity(token))
            .Returns(CreateCoreIdentityPrincipal(subject));

        // Act
        var act = () => _sut.Run(json, identity, "issuer");

        // Assert
        act.Should().Throw<SecurityTokenException>()
            .WithMessage("The core identity contains no usable name.");
        identity.HasClaim(GovUkUserClaimTypes.VerifiedIdentity, "true")
            .Should().BeFalse();
    }

    [Test]
    public void Run_AddsGivenNameAndFullName_WhenFamilyNameIsMissing()
    {
        // Arrange
        const string subject = "matching-sub";
        var token = CreateToken(subject,
            CreateName("Cher", null, "2020-01-01", null));
        var json = CreateUserInfo(token);
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, subject)
        });

        _coreIdentityHelperMock
            .Setup(h => h.ValidateCoreIdentity(token))
            .Returns(CreateCoreIdentityPrincipal(subject));

        // Act
        _sut.Run(json, identity, "issuer");

        // Assert
        identity.FindFirst(ClaimTypes.GivenName)?.Value.Should().Be("Cher");
        identity.FindFirst(ClaimTypes.Name)?.Value.Should().Be("Cher");
        identity.FindFirst(ClaimTypes.Surname).Should().BeNull();
        identity.HasClaim(GovUkUserClaimTypes.VerifiedIdentity, "true")
            .Should().BeTrue();
    }

    [Test]
    public void Run_AddsSurnameAndFullName_WhenGivenNameIsMissing()
    {
        // Arrange
        const string subject = "matching-sub";
        var token = CreateToken(subject,
            CreateName(null, "Madonna", "2020-01-01", null));
        var json = CreateUserInfo(token);
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, subject)
        });

        _coreIdentityHelperMock
            .Setup(h => h.ValidateCoreIdentity(token))
            .Returns(CreateCoreIdentityPrincipal(subject));

        // Act
        _sut.Run(json, identity, "issuer");

        // Assert
        identity.FindFirst(ClaimTypes.Surname)?.Value.Should().Be("Madonna");
        identity.FindFirst(ClaimTypes.Name)?.Value.Should().Be("Madonna");
        identity.FindFirst(ClaimTypes.GivenName).Should().BeNull();
        identity.HasClaim(GovUkUserClaimTypes.VerifiedIdentity, "true")
            .Should().BeTrue();
    }

    private static JsonElement CreateUserInfo(string token)
    {
        return JsonDocument.Parse(JsonSerializer.Serialize(
            new Dictionary<string, string>
            {
                [UserInfoClaims.CoreIdentityJWT.GetDescription()] = token
            })).RootElement;
    }

    private static ClaimsPrincipal CreateCoreIdentityPrincipal(string subject)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("sub", subject)
        }));
    }

    private static string CreateToken(string subject, params GovUkName[] names)
    {
        return CoreIdentityJwtConverter.SerializeStubCoreIdentityJwt(
            new GovUkCoreIdentityJwt
            {
                Sub = subject,
                Vc = new GovUkCoreIdentityCredential
                {
                    CredentialSubject = new GovUkCredentialSubject
                    {
                        Names = new List<GovUkName>(names)
                    }
                }
            });
    }

    private static GovUkName CreateName(
        string givenName,
        string familyName,
        string validFrom,
        string validUntil)
    {
        var parts = new List<GovUkNamePart>();

        if (givenName != null)
        {
            parts.Add(new GovUkNamePart
            {
                Type = "GivenName",
                Value = givenName
            });
        }

        if (familyName != null)
        {
            parts.Add(new GovUkNamePart
            {
                Type = "FamilyName",
                Value = familyName
            });
        }

        return new GovUkName
        {
            ValidFromRaw = validFrom,
            ValidUntilRaw = validUntil,
            NameParts = parts
        };
    }
}