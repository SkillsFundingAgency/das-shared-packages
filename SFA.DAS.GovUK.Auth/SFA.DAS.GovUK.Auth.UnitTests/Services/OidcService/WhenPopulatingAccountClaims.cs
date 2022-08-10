using System.Net;
using System.Security.Claims;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;
using Moq.Protected;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Interfaces;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.GovUK.Auth.UnitTests.Services.OidcService;

public class WhenPopulatingAccountClaims
{
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_User_Endpoint_Is_Called_Using_AccessToken_From_TokenValidatedContext(
        GovUkUser user,
        string accessToken,
        List<ClaimsIdentity> claimsIdentity,
        IOptions<GovUkOidcConfiguration> config)
    {
        //Arrange
        config.Value.BaseUrl = $"https://{config.Value.BaseUrl}";
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(claimsIdentity);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(user)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.Value.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var client = new HttpClient(httpMessageHandler.Object);
        var tokenValidatedContext = new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",","",typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), mockPrincipal.Object, new AuthenticationProperties())
        {
            TokenEndpointResponse = new OpenIdConnectMessage
            {
                Parameters = { {"access_token",accessToken} }
            },
            Principal = mockPrincipal.Object
        };
        
        var service = new Auth.Services.OidcService(client,Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config);

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);
        
        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Headers.UserAgent.FirstOrDefault(x=>
                        x.Product != null && x.Product.Version != null && x.Product.Name.Equals("DfEApprenticeships") && x.Product.Version.Equals("1")) != null
                    && c.Headers.Authorization.Scheme.Equals("Bearer")
                    && c.Headers.Authorization.Parameter.Equals(accessToken)
                ),
                ItExpr.IsAny<CancellationToken>()
            );
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_UserInfo_Endpoint_Is_Called_And_Email_Claim_Populated(
        GovUkUser user,
        string accessToken,
        List<ClaimsIdentity> claimsIdentity,
        IOptions<GovUkOidcConfiguration> config)
    {
        //Arrange
        config.Value.BaseUrl = $"https://{config.Value.BaseUrl}";
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(claimsIdentity);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(user)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.Value.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var client = new HttpClient(httpMessageHandler.Object);
        var tokenValidatedContext = new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",","",typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), mockPrincipal.Object, new AuthenticationProperties())
        {
            TokenEndpointResponse = new OpenIdConnectMessage
            {
                Parameters = { {"access_token",accessToken} }
            },
            Principal = mockPrincipal.Object
        };
        
        var service = new Auth.Services.OidcService(client,Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config);

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);
        
        //Assert
        tokenValidatedContext.Principal.Identities.First().Claims.First(c => c.Type.Equals("email")).Value.Should()
            .Be(user.Email);
    }

    private class TestAuthHandler : IAuthenticationHandler
    {
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        public Task ChallengeAsync(AuthenticationProperties? properties)
        {
            throw new NotImplementedException();
        }

        public Task ForbidAsync(AuthenticationProperties? properties)
        {
            throw new NotImplementedException();
        }
    }
}