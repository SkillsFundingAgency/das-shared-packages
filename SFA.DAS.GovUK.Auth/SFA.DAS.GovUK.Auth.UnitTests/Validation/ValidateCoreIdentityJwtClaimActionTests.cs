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
        var json = JsonDocument.Parse("{}").RootElement;
        var identity = new ClaimsIdentity();

        _sut.Run(json, identity, "issuer");

        identity.Claims.Should().BeEmpty();
        _coreIdentityHelperMock.Verify(h => h.ValidateCoreIdentity(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void Run_ThrowsException_IfSubClaimsDoNotMatch()
    {
        // Arrange
        const string token = "fake.jwt.token";
        var json = JsonDocument.Parse($@"{{""{UserInfoClaims.CoreIdentityJWT.GetDescription()}"": ""{token}""}}").RootElement;

        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "actual-sub"));

        var principalWithWrongSub = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("sub", "different-sub")
        }));

        _coreIdentityHelperMock.Setup(h => h.ValidateCoreIdentity(token)).Returns(principalWithWrongSub);

        // Act
        var act = () => _sut.Run(json, identity, "issuer");

        // Assert
        act.Should().Throw<SecurityTokenException>()
            .WithMessage("The 'sub' claim in the core identity JWT does not match the 'sub' claim from the ID token.");
    }

    [Test]
    public void Run_AddsClaim_WhenSubClaimsMatch()
    {
        // Arrange
        const string token = "valid.jwt.token";
        var json = JsonDocument.Parse($@"{{""{UserInfoClaims.CoreIdentityJWT.GetDescription()}"": ""{token}""}}").RootElement;

        var subject = "matching-sub";
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, subject));

        var coreIdentityPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("sub", subject)
        }));

        _coreIdentityHelperMock.Setup(h => h.ValidateCoreIdentity(token)).Returns(coreIdentityPrincipal);

        // Act
        _sut.Run(json, identity, "issuer");

        // Assert
        identity.HasClaim(c =>
            c.Type == UserInfoClaims.CoreIdentityJWT.GetDescription() &&
            c.Value == token &&
            c.ValueType == "JSON").Should().BeTrue();
    }
}
