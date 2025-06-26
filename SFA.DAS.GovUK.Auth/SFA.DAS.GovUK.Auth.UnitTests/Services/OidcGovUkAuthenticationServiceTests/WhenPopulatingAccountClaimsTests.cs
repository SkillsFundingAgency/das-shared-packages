using System.Net;
using System.Security.Claims;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;
using Moq.Protected;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.GovUK.Auth.UnitTests.Services.OidcGovUkAuthenticationServiceTests;

public class WhenPopulatingAccountClaimsTests
{
    [Test, MoqAutoData]
    public async Task If_Token_TokenEndpointPrincipal_Is_Null_Then_Not_Updated(GovUkOidcConfiguration config, string accessToken)
    {
        //Arrange
        var govUkUser = CreateGovUkUser();
        config.BaseUrl = $"https://{config.BaseUrl}";
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(govUkUser)),
            StatusCode = HttpStatusCode.Accepted
        };
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(new List<ClaimsIdentity>());
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var tokenValidatedContext = new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",","",typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), mockPrincipal.Object, new AuthenticationProperties())
        {
            TokenEndpointResponse = new OpenIdConnectMessage
            {
                Parameters = { {"access_token",accessToken} }
            },
            Principal = null
        };
        var service = new OidcGovUkAuthenticationService(Mock.Of<HttpClient>(),Mock.Of<ISigningCredentialsProvider>(), Mock.Of<IJwtSecurityTokenService>(), config, null);

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);
        
        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
    }
    
    [Test, MoqAutoData]
    public async Task If_Token_TokenEndpointResponse_Is_Null_Then_Not_Updated(GovUkOidcConfiguration config)
    {
        //Arrange
        var govUkUser = CreateGovUkUser();
        config.BaseUrl = $"https://{config.BaseUrl}";
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(govUkUser)),
            StatusCode = HttpStatusCode.Accepted
        };
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var tokenValidatedContext = new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",","",typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), Mock.Of<ClaimsPrincipal>(), new AuthenticationProperties())
        {
            Principal = mockPrincipal.Object
        };
        var service = new OidcGovUkAuthenticationService(Mock.Of<HttpClient>(), Mock.Of<ISigningCredentialsProvider>(), Mock.Of<IJwtSecurityTokenService>(), config, null);

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);
        
        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_User_Endpoint_Is_Called_Using_AccessToken_From_TokenValidatedContext(
        string accessToken,
        List<ClaimsIdentity> claimsIdentity,
        GovUkOidcConfiguration config)
    {
        //Arrange
        var govUkUser = CreateGovUkUser();
        config.BaseUrl = $"https://{config.BaseUrl}";
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(claimsIdentity);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(govUkUser)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
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
        
        var service = new Auth.Services.OidcGovUkAuthenticationService(client, Mock.Of<ISigningCredentialsProvider>(), Mock.Of<IJwtSecurityTokenService>(), config, Mock.Of<ICustomClaims>());

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);
        
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
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_UserInfo_Endpoint_Is_Called_And_Email_Claim_Populated(
        string accessToken,
        List<ClaimsIdentity> claimsIdentity,
        GovUkOidcConfiguration config)
    {
        //Arrange
        var govUkUser = CreateGovUkUser();
        config.BaseUrl = $"https://{config.BaseUrl}";
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(claimsIdentity);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(govUkUser)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
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
        
        var service = new Auth.Services.OidcGovUkAuthenticationService(client, Mock.Of<ISigningCredentialsProvider>(), Mock.Of<IJwtSecurityTokenService>(), config, Mock.Of<ICustomClaims>());

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);
        
        //Assert
        tokenValidatedContext.Principal.Identities.First().Claims.First(c => c.Type.Equals(ClaimTypes.Email)).Value.Should()
            .Be(govUkUser.Email);
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_UserInfo_Endpoint_Is_Called_And_Email_Claim_Populated_And_Additional_Claims_From_Function(
        string accessToken,
        string customClaimValue,
        List<ClaimsIdentity> claimsIdentity,
        GovUkOidcConfiguration config)
    {
        //Arrange
        var govUkUser = CreateGovUkUser();
        config.BaseUrl = $"https://{config.BaseUrl}";
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(claimsIdentity);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(govUkUser)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
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
        var customClaims = new Mock<ICustomClaims>();
        customClaims.Setup(x => x.GetClaims(tokenValidatedContext))
            .ReturnsAsync(new List<Claim> {new Claim("CustomClaim", customClaimValue)});
        
        var service = new OidcGovUkAuthenticationService(client, Mock.Of<ISigningCredentialsProvider>(), Mock.Of<IJwtSecurityTokenService>(), config, customClaims.Object);

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);
        
        //Assert
        tokenValidatedContext.Principal.Identities.First().Claims.First(c => c.Type.Equals(ClaimTypes.Email)).Value.Should()
            .Be(govUkUser.Email);
        tokenValidatedContext.Principal.Identities.First().Claims.First(c => c.Type.Equals("CustomClaim")).Value.Should()
            .Be(customClaimValue);
    }
    
    [Test, RecursiveMoqAutoData]
    public async Task Then_The_UserInfo_Endpoint_Is_Called_And_Email_Claim_Not_Populated_If_No_Value_Returned(
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
            Content = new StringContent(JsonSerializer.Serialize((GovUkUser)null!)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
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
        
        var service = new Auth.Services.OidcGovUkAuthenticationService(client, Mock.Of<ISigningCredentialsProvider>(), Mock.Of<IJwtSecurityTokenService>(), config, Mock.Of<ICustomClaims>());

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);
        
        //Assert
        tokenValidatedContext.Principal.Identities.First().Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email)).Should().BeNull();
    }

    private GovUkUser CreateGovUkUser()
    {
        return new GovUkUser
        {
            Email = ""
        };
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