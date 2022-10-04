using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.OidcMiddleware.GovUk.Configuration;
using SFA.DAS.OidcMiddleware.GovUk.Models;
using SFA.DAS.OidcMiddleware.GovUk.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.OidcMiddleware.GovUk.UnitTests.Services
{
    public class WhenGettingToken
    {
        [Test, MoqAutoData]
        public async Task Then_The_Endpoint_Is_Called_And_Token_Returned(
            string clientAssertion,
            Token token,
            string code,
            string redirectUri,
            GovUkOidcConfiguration config)
        {
            //Arrange
            config.BaseUrl = $"https://{config.BaseUrl}";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(token)),
                StatusCode = HttpStatusCode.Accepted
            };
            var expectedUrl = new Uri($"{config.BaseUrl}/token");
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Post);
            var client = new HttpClient(httpMessageHandler.Object);
            var jwtService = new Mock<IJwtSecurityTokenService>();
            jwtService.Setup(x => x.CreateToken(
                    It.Is<ClaimsIdentity>(c => c.HasClaim("sub", config.ClientId) && c.Claims.FirstOrDefault(f => f.Type.Equals("jti")) != null),
                    It.Is<SigningCredentials>(c => c.Kid.Equals(config.KeyVaultIdentifier) && c.Algorithm.Equals("RS512"))))
                .Returns(clientAssertion);
            var service = new OidcService(client, Mock.Of<IAzureIdentityService>(), jwtService.Object, config);

            //Act
            var actual = service.GetToken(code, redirectUri);

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Content != null
                        && c.RequestUri != null
                        && c.Method.Equals(HttpMethod.Post)
                        && c.RequestUri.Equals(expectedUrl)
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );
            actual.Should().BeEquivalentTo(token);
        }

        [Test, MoqAutoData]
        public async Task Then_The_OpenIdConnectMessage_And_Client_Assertion_Are_Passed_As_Form_Encoded_Content(
            string clientAssertion,
            Token token,
            string code,
            string redirectUri,
            GovUkOidcConfiguration config)
        {
            //Arrange
            config.BaseUrl = $"https://{config.BaseUrl}";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(token)),
                StatusCode = HttpStatusCode.Accepted
            };

            var expectedUrl = new Uri($"{config.BaseUrl}/token");
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Post);
            var client = new HttpClient(httpMessageHandler.Object);
            var jwtService = new Mock<IJwtSecurityTokenService>();
            
            jwtService.Setup(x => x.CreateToken(
                    It.Is<ClaimsIdentity>(c => c.HasClaim("sub", config.ClientId) && c.Claims.FirstOrDefault(f => f.Type.Equals("jti")) != null),
                    It.Is<SigningCredentials>(c => c.Kid.Equals(config.KeyVaultIdentifier) && c.Algorithm.Equals("RS512"))))
                .Returns(clientAssertion);
            var service = new OidcService(client, Mock.Of<IAzureIdentityService>(), jwtService.Object, config);

            //Act
            service.GetToken(code, redirectUri);

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Content != null
                        && c.Headers.Accept.Any(x => x.MediaType != null && x.MediaType.Equals("application/x-www-form-urlencoded"))
                        && c.Headers.Accept.Any(x => x.MediaType != null && x.MediaType.Equals("*/*"))
                        && c.Headers.UserAgent.FirstOrDefault(x =>
                            x.Product != null && x.Product.Version != null && x.Product.Name.Equals("DfEApprenticeships") && x.Product.Version.Equals("1")) != null
                        && c.Content.Headers.Count() == 1
                        && c.Content.Headers.Any(x => x.Key.Equals("Content-Type") && x.Value.First().Equals("application/x-www-form-urlencoded"))
                        && c.Content.ReadAsStringAsync().Result.Contains("grant_type=authorization_code")
                        && c.Content.ReadAsStringAsync().Result.Contains("client_assertion_type=urn%3Aietf%3Aparams%3Aoauth%3Aclient-assertion-type%3Ajwt-bearer")
                        && c.Content.ReadAsStringAsync().Result.Contains($"redirect_uri={redirectUri}")
                        && c.Content.ReadAsStringAsync().Result.Contains($"code={code}")
                        && c.Content.ReadAsStringAsync().Result.Contains($"client_assertion={clientAssertion}")
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );
        }
    }
}
