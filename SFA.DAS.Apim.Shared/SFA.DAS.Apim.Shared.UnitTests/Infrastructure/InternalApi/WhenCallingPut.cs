using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using Moq.Protected;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.Apim.Shared.Infrastructure;
using SFA.DAS.Apim.Shared.Interfaces;

namespace SFA.DAS.Apim.Shared.UnitTests.Infrastructure.InternalApi
{
    public class WhenCallingPut
    {
        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called(
            string authToken,
            string postContent,
            int id,
            TestInternalApiConfiguration config)
        {
            //Arrange
            var azureClientCredentialHelper = new Mock<IAzureClientCredentialHelper>();
            azureClientCredentialHelper.Setup(x => x.GetAccessTokenAsync(config.Identifier)).ReturnsAsync(authToken);
            config.Url = "https://test.local";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.NoContent
            };
            var putTestRequest = new PutTestRequest(id) {Data = postContent};
            var expectedUrl = $"{config.Url}{putTestRequest.PutUrl}";
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, "put");
            var client = new HttpClient(httpMessageHandler.Object);
            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            
            var actual = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object, config, azureClientCredentialHelper.Object);

            //Act
            await actual.Put(putTestRequest);

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Put)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.FirstOrDefault(h=>h.Key.Equals("X-Version")).Value.FirstOrDefault() == "2.0"
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );
        }
        
        private class PutTestRequest : IPutApiRequest
        {
            private readonly int _id;

            public string Version => "2.0";

            public PutTestRequest(int id)
            {
                _id = id;
            }
            public object Data { get; set; }
            public string PutUrl => $"/test-url/put{_id}";
        }
    }
}