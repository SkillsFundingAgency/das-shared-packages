using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using Moq.Protected;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.Apim.Shared.Infrastructure;
using SFA.DAS.Apim.Shared.Interfaces;

namespace SFA.DAS.Apim.Shared.UnitTests.Infrastructure.InternalApi;

public class WhenCallingGetResponseCode
{
    [Test, AutoData]
    public async Task Then_The_Endpoint_Is_Called_And_StatusCode_Returned(
        string authToken,
        int id,
        HttpStatusCode code,
        TestInternalApiConfiguration config)
    {
        //Arrange
        var azureClientCredentialHelper = new Mock<IAzureClientCredentialHelper>();
        azureClientCredentialHelper.Setup(x => x.GetAccessTokenAsync(config.Identifier)).ReturnsAsync(authToken);
        config.Url = "https://test.local";
        var response = new HttpResponseMessage
        {
            Content = new StringContent(""),
            StatusCode = code
        };
        var getTestRequest = new GetTestRequest(id) ;
        var expectedUrl = $"{config.Url}{getTestRequest.GetUrl}";
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl);
        var client = new HttpClient(httpMessageHandler.Object);
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        var actual = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object, config, azureClientCredentialHelper.Object);

        //Act
        var actualResult = await actual.GetResponseCode(getTestRequest);

        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                    && c.Headers.Authorization.Scheme.Equals("Bearer")
                    && c.Headers.FirstOrDefault(h=>h.Key.Equals("X-Version")).Value.FirstOrDefault() == "2.0"
                    && c.Headers.Authorization.Parameter.Equals(authToken)),
                ItExpr.IsAny<CancellationToken>()
            );
        actualResult.Should().Be(code);
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
            Content = new StringContent(""),
            StatusCode = HttpStatusCode.Accepted
        };
        var getTestRequest = new GetTestRequest(id);
        var expectedUrl = $"{config.Url}{getTestRequest.GetUrl}";
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl);
        var client = new HttpClient(httpMessageHandler.Object);
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        var actual = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object,configuration, Mock.Of<IAzureClientCredentialHelper>());

        //Act
        await actual.GetResponseCode(getTestRequest);
             
        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    && c.Headers.FirstOrDefault(h=>h.Key.Equals("X-Version")).Value.FirstOrDefault() == "2.0"
                    && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                    && c.Headers.Authorization == null),
                ItExpr.IsAny<CancellationToken>()
            );
    }
    private class GetTestRequest : IGetApiRequest
    {
        private readonly int _id;

        public string Version => "2.0";

        public GetTestRequest (int id)
        {
            _id = id;
        }
             
        public string GetUrl => $"/test-url/get{_id}";
    }
}