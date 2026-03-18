using System.Collections.Generic;
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
    public class WhenCallingGetAll
    {
        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called(
            string authToken,
            TestInternalApiConfiguration config)
        {
            //Arrange
            var azureClientCredentialHelper = new Mock<IAzureClientCredentialHelper>();
            azureClientCredentialHelper.Setup(x => x.GetAccessTokenAsync(config.Identifier)).ReturnsAsync(authToken);
            config.Url = "https://test.local";
            var configuration = config;
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<string>{"string","string"})),
                StatusCode = HttpStatusCode.Accepted
            };
            var getTestRequest = new GetAllTestRequest();
            var expectedUrl = $"{config.Url}{getTestRequest.GetAllUrl}";
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl);
            var client = new HttpClient(httpMessageHandler.Object);
            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            var apiClient = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object, configuration, azureClientCredentialHelper.Object);

            //Act
            var actual = await apiClient.GetAll<string>(getTestRequest);

            Assert.That(actual, Is.AssignableFrom<List<string>>());
            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        [Test, AutoData]
         public async Task Then_The_Bearer_Token_Is_Not_Added_If_Local(
             TestInternalApiConfiguration config)
         {
             //Arrange
             config.Url = "https://test.local";
             config.Identifier = "";
             var configuration = config;
             var response = new HttpResponseMessage
             {
                 Content = new StringContent("[\"test\"]"),
                 StatusCode = HttpStatusCode.Accepted
             };
             var getTestRequest = new GetAllTestRequest();
             var expectedUrl = $"{config.Url}{getTestRequest.GetAllUrl}";
             var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl);
             var client = new HttpClient(httpMessageHandler.Object);
             var clientFactory = new Mock<IHttpClientFactory>();
             clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
             var actual = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object,configuration, Mock.Of<IAzureClientCredentialHelper>());

             //Act
             await actual.GetAll<string>(getTestRequest);
             
             //Assert
             httpMessageHandler.Protected()
                 .Verify<Task<HttpResponseMessage>>(
                     "SendAsync", Times.Once(),
                     ItExpr.Is<HttpRequestMessage>(c =>
                         c.Method.Equals(HttpMethod.Get)
                         && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                         && c.Headers.Authorization == null),
                     ItExpr.IsAny<CancellationToken>()
                 );
         }

         [Test, AutoData]
         public async Task Then_If_Returns_Not_Found_Result_Returns_Empty_List(
             TestInternalApiConfiguration config)
         {
             //Arrange
             config.Url = "https://test.local";
             var configuration = config;
             var response = new HttpResponseMessage
             {
                 Content = new StringContent(""),
                 StatusCode = HttpStatusCode.NotFound
             };
             var getTestRequest = new GetAllTestRequest();
             var expectedUrl = $"{config.Url}{getTestRequest.GetAllUrl}";
             var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl);
             var client = new HttpClient(httpMessageHandler.Object);
             var clientFactory = new Mock<IHttpClientFactory>();
             clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
             var actual = new InternalApiClient<TestInternalApiConfiguration>(clientFactory.Object,configuration, Mock.Of<IAzureClientCredentialHelper>());

             //Act
             var actualResult = await actual.GetAll<string>(getTestRequest);
             
             //Assert
             Assert.That(actualResult, Is.Empty);
         }
        
        private class GetAllTestRequest : IGetAllApiRequest
        {
            public string GetAllUrl => "/test-url/get-all";
        }
        
    }
}