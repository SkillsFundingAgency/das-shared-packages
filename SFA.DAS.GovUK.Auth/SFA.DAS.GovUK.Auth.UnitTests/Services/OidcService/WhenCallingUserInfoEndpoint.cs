using System.Net;
using System.Security.Claims;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.GovUK.Auth.UnitTests.Services.OidcService;

public class WhenCallingUserInfoEndpoint
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_User_Endpoint_Is_Called_Using_AccessToken_From_TokenValidatedContext(
        GovUkUser user,
        string accessToken,
        List<ClaimsIdentity> claimsIdentity,
        GovUkOidcConfiguration config)
    {
        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(claimsIdentity);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(user)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var client = new HttpClient(httpMessageHandler.Object);
        
        var service = new Auth.Services.OidcService(client,Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config, Mock.Of<ICustomClaims>());

        //Act
        var details = await service.GetAccountDetails(accessToken);
        
        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Headers.Authorization != null
                    && c.Headers.Authorization.Parameter != null
                    && c.Headers.UserAgent.FirstOrDefault(x=>
                        x.Product != null && x.Product.Version != null && x.Product.Name.Equals("DfEApprenticeships") && x.Product.Version.Equals("1")) != null 
                    && c.Headers.Authorization.Scheme.Equals("Bearer") 
                    && c.Headers.Authorization.Parameter.Equals(accessToken)
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        details.Should().BeEquivalentTo(user);
    }
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_User_Endpoint_Is_Called_Using_AccessToken_From_TokenValidatedContext_And_If_Not_Found_Null_Returned(
        string accessToken,
        List<ClaimsIdentity> claimsIdentity,
        GovUkOidcConfiguration config)
    {
        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(claimsIdentity);
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        };
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var client = new HttpClient(httpMessageHandler.Object);
        
        var service = new Auth.Services.OidcService(client,Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config, Mock.Of<ICustomClaims>());

        //Act
        var details = await service.GetAccountDetails(accessToken);
        
        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Headers.Authorization != null
                    && c.Headers.Authorization.Parameter != null
                    && c.Headers.UserAgent.FirstOrDefault(x=>
                        x.Product != null && x.Product.Version != null && x.Product.Name.Equals("DfEApprenticeships") && x.Product.Version.Equals("1")) != null 
                    && c.Headers.Authorization.Scheme.Equals("Bearer") 
                    && c.Headers.Authorization.Parameter.Equals(accessToken)
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        details.Should().BeNull();
    }
}