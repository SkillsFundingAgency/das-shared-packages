using System.Net;
using FluentAssertions;
using Moq;
using Moq.Protected;
using SFA.DAS.DfESignIn.Auth.Api.Client;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Api.Models;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Client
{
    [TestFixture]
    public class DfeSignInApiHelperTest
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private Mock<ITokenBuilder> _tokenBuilder;

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

            // use real http client with mocked handler here
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var subjectUnderTest = new DfeSignInApiHelper(httpClient, _tokenBuilder.Object);
            _tokenBuilder.Setup(x => x.CreateToken()).Returns(authToken.ToString);

            // ACT
            var result = await subjectUnderTest.Get<ApiServiceResponse>("api/test/whatever");

            // ASSERT
            result.Should().NotBeNull(); // this is fluent assertions here...
            result.UserId.Should().Be(userId);
            result.ServiceId.Should().Be(serviceId);
            result.OrganisationId.Should().Be(orgId);

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri("http://test.com/api/test/whatever");

            // verify if called at least once.
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1), // we expected a single external request
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get  // we expected a GET request
                        && req.RequestUri == expectedUri
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

            // use real http client with mocked handler here
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };
            
            var subjectUnderTest = new DfeSignInApiHelper(httpClient, _tokenBuilder.Object);

            // ACT
            var result = await subjectUnderTest.Get<ApiServiceResponse>("api/test/whatever");

            // ASSERT
            result.Should().BeNull(); // this is fluent assertions here...

            // also check the 'http' call was like we expected it
            var expectedUri = new Uri("http://test.com/api/test/whatever");

            // verify if called at least once.
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1), // we expected a single external request
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get  // we expected a GET request
                        && req.RequestUri == expectedUri // to this uri
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
