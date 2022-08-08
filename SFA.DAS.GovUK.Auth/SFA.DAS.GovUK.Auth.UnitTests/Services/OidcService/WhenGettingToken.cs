using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;
using Moq.Protected;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.GovUK.Auth.UnitTests.Services.OidcService;

public class WhenGettingToken
{
    [Test, MoqAutoData]
    public void Then_If_The_Config_Is_Not_Valid_Then_Exception_Thrown(
        string clientAssertion,
        Token token,
        OpenIdConnectMessage openIdConnectMessage,
        IOptions<GovUkOidcConfiguration> config)
    {
        config.Value.BaseUrl = null;
        var client = new HttpClient(Mock.Of<HttpMessageHandler>());

        Assert.Throws<ArgumentException>(() =>
        {
            _ = new Auth.Services.OidcService(client, config);
        });
    }
    
    [Test, MoqAutoData]
    public async Task Then_The_Endpoint_Is_Called_And_Token_Returned(
        string clientAssertion,
        Token token,
        OpenIdConnectMessage openIdConnectMessage,
        IOptions<GovUkOidcConfiguration> config)
    {
        //Arrange
        config.Value.BaseUrl = $"https://{config.Value.BaseUrl}";
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(token)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.Value.BaseUrl}/token");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Post);
        var client = new HttpClient(httpMessageHandler.Object);
        var service = new Auth.Services.OidcService(client, config);
        
        //Act
        var actual = await service.GetToken(openIdConnectMessage, clientAssertion);
        
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
        OpenIdConnectMessage openIdConnectMessage,
        IOptions<GovUkOidcConfiguration> config)
    {
        //Arrange
        config.Value.BaseUrl = $"https://{config.Value.BaseUrl}";
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(token)),
            StatusCode = HttpStatusCode.Accepted
        };
        
        var expectedUrl = new Uri($"{config.Value.BaseUrl}/token");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Post);
        var client = new HttpClient(httpMessageHandler.Object);
        var service = new Auth.Services.OidcService(client, config);
        
        //Act
        await service.GetToken(openIdConnectMessage, clientAssertion);
        
        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Content != null
                    && c.Headers.Accept.Any(x=>x.MediaType != null && x.MediaType.Equals("application/x-www-form-urlencoded")) 
                    && c.Headers.Accept.Any(x=>x.MediaType != null && x.MediaType.Equals("*/*")) 
                    && c.Headers.UserAgent.FirstOrDefault(x=>
                        x.Product != null && x.Product.Version != null && x.Product.Name.Equals("DfEApprenticeships") && x.Product.Version.Equals("1")) != null 
                    && c.Content.Headers.Count() == 1 
                    && c.Content.Headers.Any(x=>x.Key.Equals("Content-Type") && x.Value.First().Equals("application/x-www-form-urlencoded"))
                    && c.Content.ReadAsStringAsync().Result.Contains("grant_type=authorization_code")
                    && c.Content.ReadAsStringAsync().Result.Contains("client_assertion_type=urn%3Aietf%3Aparams%3Aoauth%3Aclient-assertion-type%3Ajwt-bearer")
                    && c.Content.ReadAsStringAsync().Result.Contains($"redirect_uri={openIdConnectMessage.RedirectUri}", StringComparison.CurrentCultureIgnoreCase)
                    && c.Content.ReadAsStringAsync().Result.Contains($"code={openIdConnectMessage.Code}", StringComparison.CurrentCultureIgnoreCase)
                    && c.Content.ReadAsStringAsync().Result.Contains($"client_assertion={clientAssertion}", StringComparison.CurrentCultureIgnoreCase)
                ),
                ItExpr.IsAny<CancellationToken>()
            );
    }
}