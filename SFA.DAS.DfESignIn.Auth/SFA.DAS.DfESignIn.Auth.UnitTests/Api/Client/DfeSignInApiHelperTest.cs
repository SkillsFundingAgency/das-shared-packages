using System.Net;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Api.Client;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Api.Models;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Client
{
    [TestFixture]
    public class DfeSignInApiHelperTest
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler = null!;
        private Mock<ITokenBuilder> _tokenBuilder = null!;

        [SetUp]
        public void SetUp()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _tokenBuilder = new Mock<ITokenBuilder>();
        }

        [Test]
        public async Task Get_Return_Expected_When_HttpResponse_IsValid()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var orgId = Guid.NewGuid();
            var authToken = Guid.NewGuid();

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"{{'userId':'{userId}','serviceId':'{serviceId}', 'organisationId':'{orgId}'}}"),
                })
                .Verifiable();

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var subjectUnderTest = new DfeSignInApiHelper(httpClient, _tokenBuilder.Object);
            _tokenBuilder.Setup(x => x.CreateToken()).Returns(authToken.ToString);

            // ACT
            var result = await subjectUnderTest.Get<ApiServiceResponse>("api/test/whatever");

            // ASSERT
            result.Should().NotBeNull();
            result!.UserId.Should().Be(userId);
            result.ServiceId.Should().Be(serviceId);
            result.OrganisationId.Should().Be(orgId);

            var expectedUri = new Uri("http://test.com/api/test/whatever");

            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get
                        && req.RequestUri == expectedUri
                    && req.Headers.Authorization != null
                    && req.Headers.Authorization.Scheme == "Bearer"
                    && req.Headers.Authorization.Parameter == authToken.ToString()
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task Get_Return_Default_When_HttpResponse_IsInValid()
        {
            // ARRANGE
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = null,
                })
                .Verifiable();

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };
            
            var subjectUnderTest = new DfeSignInApiHelper(httpClient, _tokenBuilder.Object);

            // ACT
            var result = await subjectUnderTest.Get<ApiServiceResponse>("api/test/whatever");

            // ASSERT
            result.Should().BeNull();

            var expectedUri = new Uri("http://test.com/api/test/whatever");

            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get
                        && req.RequestUri == expectedUri
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task Then_If_Response_Is_Valid_Then_Returns_Response()
        {
            // ARRANGE
            var response = new ApiResponse<string>("test");
            var authToken = Guid.NewGuid();

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                })
                .Verifiable();

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _tokenBuilder.Setup(x => x.CreateToken()).Returns(authToken.ToString);
            var subjectUnderTest = new DfeSignInApiHelper(httpClient, _tokenBuilder.Object);

            // ACT
            var actual = await subjectUnderTest.Get<ApiResponse<string>>("https://test.local/api/test");

            // ASSERT
            actual.Should().NotBeNull();
            actual!.Body.Should().Be("test");
        }
    }
}
