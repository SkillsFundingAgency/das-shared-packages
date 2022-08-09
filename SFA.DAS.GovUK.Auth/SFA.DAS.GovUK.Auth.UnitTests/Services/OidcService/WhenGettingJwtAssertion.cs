using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.GovUK.Auth.UnitTests.Services.OidcService;

public class WhenGettingJwtAssertion
{
    [Test, MoqAutoData]
    public void Then_The_Token_Service_Is_Called_And_Assertion_Returned(
        string data,
        string issuer, 
        string audience,
        IOptions<GovUkOidcConfiguration> config)
    {
        //Arrange
        config.Value.BaseUrl = $"https://{config.Value.BaseUrl}";
        var jwtService = new Mock<IJwtSecurityTokenService>();
        jwtService.Setup(x => x.CreateToken(config.Value.ClientId, $"{config.Value.BaseUrl}/token", 
            It.Is<ClaimsIdentity>(c=>c.HasClaim("sub",config.Value.ClientId) && c.Claims.FirstOrDefault(f=>f.Type.Equals("jti"))!=null),
            It.Is<SigningCredentials>(c=>c.Kid.Equals(config.Value.KeyVaultIdentifier) && c.Algorithm.Equals("RS512"))))
            .Returns(data);
        var service = new Auth.Services.OidcService(Mock.Of<HttpClient>(),Mock.Of<IAzureIdentityService>(), jwtService.Object, config);

        //Act
        var actual = service.CreateJwtAssertion();

        //Assert
        actual.Should().BeEquivalentTo(data);
    }
}