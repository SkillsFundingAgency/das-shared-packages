using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Moq.Protected;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.SharedOuterApi.Infrastructure;
using SFA.DAS.SharedOuterApi.Interfaces;

namespace SFA.DAS.SharedOuterApi.UnitTests.Infrastructure.InternalApi;

public class WhenCallingPatch
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
        var patchTestRequest = new PatchTestRequest(id) { Data = postContent };
        var expectedUrl = $"{config.Url}{patchTestRequest.PatchUrl}";
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, "patch");
        var client = new HttpClient(httpMessageHandler.Object);
        var hostingEnvironment = new Mock<IWebHostEnvironment>();
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

        hostingEnvironment.Setup(x => x.EnvironmentName).Returns("Staging");
        var actual = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object, config, azureClientCredentialHelper.Object);

        //Act
        await actual.Patch(patchTestRequest);

        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Patch)
                    && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                    && c.Headers.Authorization.Scheme.Equals("Bearer")
                    && c.Headers.FirstOrDefault(h => h.Key.Equals("X-Version")).Value.FirstOrDefault() == "2.0"
                    && c.Headers.Authorization.Parameter.Equals(authToken)),
                ItExpr.IsAny<CancellationToken>()
            );
    }

    private class PatchTestRequest : IPatchApiRequest<string>
    {
        private readonly int _id;

        public string Version => "2.0";

        public PatchTestRequest(int id)
        {
            _id = id;
        }
        public string Data { get; set; }
        public string PatchUrl => $"/test-url/patch{_id}";
    }
}