using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Moq.Protected;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.SharedOuterApi.Infrastructure;
using SFA.DAS.SharedOuterApi.Interfaces;

namespace SFA.DAS.SharedOuterApi.UnitTests.Infrastructure.InternalApi
{
    public class WhenCallingPostWithResponseCode
    {
        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called_And_Content_Returned_With_Response_Code(
            string authToken,
            string postContent,
            int id,
            string responseContent,
            TestInternalApiConfiguration config)
        {
            //Arrange
            var azureClientCredentialHelper = new Mock<IAzureClientCredentialHelper>();
            azureClientCredentialHelper.Setup(x => x.GetAccessTokenAsync(config.Identifier)).ReturnsAsync(authToken);
            config.Url = "https://test.local";
            var testObject = JsonSerializer.Serialize(new TestResponse{MyResponse = responseContent});
            var response = new HttpResponseMessage
            {
                Content = new StringContent(testObject),
                StatusCode = HttpStatusCode.Created
            };
            var postTestRequest = new PostTestRequest(id) {Data = postContent};
            var expectedUrl = $"{config.Url}{postTestRequest.PostUrl}";
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, "post");
            var client = new HttpClient(httpMessageHandler.Object);
            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            
            var actual = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object, config, azureClientCredentialHelper.Object);

            //Act
            var actualResult = await actual.PostWithResponseCode<TestResponse>(postTestRequest);

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Post)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.FirstOrDefault(h=>h.Key.Equals("X-Version")).Value.FirstOrDefault() == "2.0"
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );
            
            actualResult.StatusCode.Should().Be(HttpStatusCode.Created);
            actualResult.Body.MyResponse.Should().Be(responseContent);
        }
        
        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called_And_No_Content_Returned_If_Flag_Set_With_Response_Code(
            string authToken,
            string postContent,
            int id,
            string responseContent,
            TestInternalApiConfiguration config)
        {
            //Arrange
            var azureClientCredentialHelper = new Mock<IAzureClientCredentialHelper>();
            azureClientCredentialHelper.Setup(x => x.GetAccessTokenAsync(config.Identifier)).ReturnsAsync(authToken);
            config.Url = "https://test.local";
            var testObject = JsonSerializer.Serialize(new TestResponse{MyResponse = responseContent});
            var response = new HttpResponseMessage
            {
                Content = new StringContent(testObject),
                StatusCode = HttpStatusCode.Created
            };
            var postTestRequest = new PostTestRequest(id) {Data = postContent};
            var expectedUrl = $"{config.Url}{postTestRequest.PostUrl}";
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, "post");
            var client = new HttpClient(httpMessageHandler.Object);
            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            
            var actual = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object, config, azureClientCredentialHelper.Object);

            //Act
            var actualResult = await actual.PostWithResponseCode<TestResponse>(postTestRequest, false);

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Post)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.FirstOrDefault(h=>h.Key.Equals("X-Version")).Value.FirstOrDefault() == "2.0"
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );
            
            actualResult.StatusCode.Should().Be(HttpStatusCode.Created);
            actualResult.Body.Should().BeNull();
        }
        
        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called_And_ErrorDescription_Returned_With_Response_Code_If_Not_Created(
            string authToken,
            string postContent,
            int id,
            string responseContent,
            TestInternalApiConfiguration config)
        {
            //Arrange
            var azureClientCredentialHelper = new Mock<IAzureClientCredentialHelper>();
            azureClientCredentialHelper.Setup(x => x.GetAccessTokenAsync(config.Identifier)).ReturnsAsync(authToken);
            config.Url = "https://test.local";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(responseContent),
                StatusCode = HttpStatusCode.BadRequest
            };
            var postTestRequest = new PostTestRequest(id) {Data = postContent};
            var expectedUrl = $"{config.Url}{postTestRequest.PostUrl}";
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, "post");
            var client = new HttpClient(httpMessageHandler.Object);
            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            
            var actual = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object, config, azureClientCredentialHelper.Object);

            //Act
            var actualResult = await actual.PostWithResponseCode<TestResponse>(postTestRequest);

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Post)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.FirstOrDefault(h=>h.Key.Equals("X-Version")).Value.FirstOrDefault() == "2.0"
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );
            
            actualResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actualResult.Body.Should().BeNull();
            actualResult.ErrorContent.Should().Be(responseContent);
        }
        
        private class PostTestRequest : IPostApiRequest
        {
            private readonly int _id;

            public string Version => "2.0";

            public PostTestRequest (int id)
            {
                _id = id;
            }
            public object Data { get; set; }
            public string PostUrl => $"/test-url/get{_id}";
        }

        private class TestResponse
        {
            public string MyResponse { get; set; }
        }
    }
}