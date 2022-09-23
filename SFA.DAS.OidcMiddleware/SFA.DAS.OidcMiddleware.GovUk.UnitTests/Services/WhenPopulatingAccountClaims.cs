using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.OidcMiddleware.GovUk.Configuration;
using SFA.DAS.OidcMiddleware.GovUk.Models;
using SFA.DAS.OidcMiddleware.GovUk.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.OidcMiddleware.GovUk.UnitTests.Services
{
    public class WhenPopulatingAccountClaims
    {
        [Test, MoqAutoData]
        public void If_Token_TokenEndpointPrincipal_Is_Null_Then_Not_Updated(GovUkUser user, GovUkOidcConfiguration config, string accessToken)
        {
            //Arrange
            config.BaseUrl = $"https://{config.BaseUrl}";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(user)),
                StatusCode = HttpStatusCode.Accepted
            };
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(x => x.Identities).Returns(new List<ClaimsIdentity>());
            var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
            var service = new OidcService(Mock.Of<HttpClient>(),Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config);

            //Act
            service.PopulateAccountClaims(null, accessToken);
        
            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Never(),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                );
        }
    
        [Test, MoqAutoData]
        public void If_Token_TokenEndpointResponse_Is_Null_Then_Not_Updated(GovUkUser user, GovUkOidcConfiguration config)
        {
            //Arrange
            config.BaseUrl = $"https://{config.BaseUrl}";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(user)),
                StatusCode = HttpStatusCode.Accepted
            };
            var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
            var service = new OidcService(Mock.Of<HttpClient>(),Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config);

            //Act
            service.PopulateAccountClaims(new ClaimsIdentity(), "");
        
            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Never(),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                );
        }
    
        [Test, RecursiveMoqAutoData]
        public void Then_The_User_Endpoint_Is_Called_Using_AccessToken_From_TokenValidatedContext(
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
            
        
            var service = new OidcService(client,Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config);

            //Act
            service.PopulateAccountClaims(new ClaimsIdentity(), accessToken);
        
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
        public void Then_The_UserInfo_Endpoint_Is_Called_And_Email_Claim_Populated(
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
            
        
            var service = new OidcService(client,Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config);

            //Act
            var identity = new ClaimsIdentity();
            service.PopulateAccountClaims(identity, accessToken);
        
            //Assert
            identity.Claims.First(c => c.Type.Equals(ClaimTypes.Email)).Value.Should().Be(user.Email);
            identity.Claims.First(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value.Should().Be(user.Sub);
        }
    
        [Test, RecursiveMoqAutoData]
        public void Then_The_UserInfo_Endpoint_Is_Called_And_Email_Claim_Not_Populated_If_No_Value_Returned(
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
                Content = new StringContent(JsonSerializer.Serialize((GovUkUser)null)),
                StatusCode = HttpStatusCode.Accepted
            };
            var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
            var client = new HttpClient(httpMessageHandler.Object);
            
            var service = new OidcService(client,Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config);

            //Act
            var identity = new ClaimsIdentity();
            service.PopulateAccountClaims(identity, accessToken);
        
            //Assert
            identity.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email)).Should().BeNull();
            identity.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Should().BeNull();
        }
        
    }
}