using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using Moq.Protected;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.SharedOuterApi.Infrastructure;
using SFA.DAS.SharedOuterApi.Interfaces;

namespace SFA.DAS.SharedOuterApi.UnitTests.Infrastructure.InternalApi;

public class WhenCallingGetWithResponseCode
{
    [Test, AutoData]
    public async Task Then_The_Endpoint_Is_Called(
        string authToken,
        int id,
        TestInternalApiConfiguration config)
    {
        //Arrange
        var azureClientCredentialHelper = new Mock<IAzureClientCredentialHelper>();
        azureClientCredentialHelper.Setup(x => x.GetAccessTokenAsync(config.Identifier)).ReturnsAsync(authToken);
        config.Url = "https://test.local";
        var response = new HttpResponseMessage
        {
            Content = new StringContent("\"test\""),
            StatusCode = HttpStatusCode.Accepted
        };
        var getTestRequest = new GetTestRequest(id);
        var expectedUrl = $"{config.Url}/{getTestRequest.GetUrl}";
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl);
        var client = new HttpClient(httpMessageHandler.Object);
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        var actualClient = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object, config, azureClientCredentialHelper.Object);

        //Act
        var actual = await actualClient.GetWithResponseCode<string>(getTestRequest);

        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                    && c.Headers.Authorization.Scheme.Equals("Bearer")
                    && c.Headers.FirstOrDefault(h => h.Key.Equals("X-Version")).Value.FirstOrDefault() == "2.0"
                    && c.Headers.Authorization.Parameter.Equals(authToken)),
                ItExpr.IsAny<CancellationToken>()
            );
        actual.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Test, AutoData]
    public async Task Then_Multiple_Calls_Do_Not_Result_In_Multiple_Versions_Added(string authToken,
        int id,
        TestInternalApiConfiguration config)
    {
        //Arrange
        var azureClientCredentialHelper = new Mock<IAzureClientCredentialHelper>();
        azureClientCredentialHelper.Setup(x => x.GetAccessTokenAsync(config.Identifier)).ReturnsAsync(authToken);
        config.Url = "https://test.local";
            
        var response = new HttpResponseMessage
        {
            Content = new StringContent("\"test\""),
            StatusCode = HttpStatusCode.Accepted
        };
        var getTestRequest = new GetTestRequest(id);
        var expectedUrl = $"{config.Url}/{getTestRequest.GetUrl}";
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl);
        var client = new HttpClient(httpMessageHandler.Object);
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        var actual = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object, config, azureClientCredentialHelper.Object);

        //Act
        await actual.GetWithResponseCode<string>(getTestRequest);
        await actual.GetWithResponseCode<string>(getTestRequest);

        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Exactly(2),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                    && c.Headers.Authorization.Scheme.Equals("Bearer")
                    && c.Headers.FirstOrDefault(h => h.Key.Equals("X-Version")).Value.Single() == "2.0"
                    && c.Headers.Authorization.Parameter.Equals(authToken)),
                ItExpr.IsAny<CancellationToken>()
            );
    }

    [Test, AutoData]
    public async Task Then_The_Bearer_Token_Is_Not_Added_If_Local_And_Default_Version_If_Not_Specified(
        int id,
        TestInternalApiConfiguration config)
    {
        //Arrange
        config.Url = "https://test.local";
        config.Identifier = "";
        var configuration = config;
        var response = new HttpResponseMessage
        {
            Content = new StringContent("\"test\""),
            StatusCode = HttpStatusCode.Accepted
        };
        var getTestRequest = new GetTestRequestNoVersion(id);
        var expectedUrl = $"{config.Url}/{getTestRequest.GetUrl}";
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl);
        var client = new HttpClient(httpMessageHandler.Object);
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
             
        var actual = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object,configuration, Mock.Of<IAzureClientCredentialHelper>());

        //Act
        await actual.GetWithResponseCode<string>(getTestRequest);
             
        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    && c.Headers.FirstOrDefault(h=>h.Key.Equals("X-Version")).Value.FirstOrDefault() == "1.0"
                    && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                    && c.Headers.Authorization == null),
                ItExpr.IsAny<CancellationToken>()
            );
    }

    [Test, AutoData]
    public async Task Then_If_Returns_Error_Result_Returns_ErrorCode_With_Response(string responseContent, int id,
        TestInternalApiConfiguration config)
    {
        //Arrange
        config.Url = "https://test.local";
        config.Identifier = "";
        var configuration = config;
        var response = new HttpResponseMessage
        {
            Content = new StringContent(responseContent),
            StatusCode = HttpStatusCode.TooManyRequests
        };
        var getTestRequest = new GetTestRequestNoVersion(id);
        var expectedUrl = $"{config.Url}/{getTestRequest.GetUrl}";
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl);
        var client = new HttpClient(httpMessageHandler.Object);
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
             
        var actual = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object,configuration, Mock.Of<IAzureClientCredentialHelper>());

        //Act
        var actualResult = await actual.GetWithResponseCode<string>(getTestRequest);
             
        //Assert
        Assert.That(actualResult, Is.Not.Null);
        actualResult.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        actualResult.Body.Should().BeNull();
        actualResult.ErrorContent.Should().Be(responseContent);
    }

    [Test, AutoData]
    public async Task Then_The_Casing_Is_Ignored_On_Deserialization(int id,string authToken,string responseValue,
        TestInternalApiConfiguration config)
    {
        //Arrange
        var azureClientCredentialHelper = new Mock<IAzureClientCredentialHelper>();
        azureClientCredentialHelper.Setup(x => x.GetAccessTokenAsync(config.Identifier)).ReturnsAsync(authToken);
        config.Url = "https://test.local";
        var response = new HttpResponseMessage
        {
            Content = new StringContent("{\"SOMEID\":\"" + responseValue +"\"}"),
            StatusCode = HttpStatusCode.Accepted
        };
        var getTestRequest = new GetTestRequest(id);
        var expectedUrl = $"{config.Url}/{getTestRequest.GetUrl}";
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl);
        var client = new HttpClient(httpMessageHandler.Object);
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        var actualClient = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object, config, azureClientCredentialHelper.Object);
            
        //Act
        var actualResult = await actualClient.GetWithResponseCode<TestClass>(getTestRequest);
             
        //Assert
        Assert.That(actualResult, Is.Not.Null);
        actualResult.StatusCode.Should().Be(HttpStatusCode.Accepted);
        actualResult.Body.SomeId.Should().Be(responseValue);
    }
        
    private class GetTestRequest : IGetApiRequest
    {
        private readonly int _id;

        public string Version => "2.0";

        public GetTestRequest (int id)
        {
            _id = id;
        }
        public string GetUrl => $"test-url/get{_id}";
    }
    private class GetTestRequestNoVersion : IGetApiRequest
    {
        private readonly int _id;

        public GetTestRequestNoVersion (int id)
        {
            _id = id;
        }
        public string GetUrl => $"test-url/get{_id}";
    }

    private class TestClass
    {
        public string SomeId { get; set; }
    }
}