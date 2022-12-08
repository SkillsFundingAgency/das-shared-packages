using System.Net;
using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.Protected;
using SFA.DAS.DfESignIn.Auth.Api.Client;
using SFA.DAS.DfESignIn.Auth.Api.Models;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Client
{
    [TestFixture]
    public class DfeSignInApiHelperTest
    {
        private MockRepository _mockRepository;
        private Mock<HttpClient> _mockHttpClient;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockHttpClient = _mockRepository.Create<HttpClient>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        }

        [TestCase("")]
        [TestCase(null)]
        public void Get_ThrowsException_Missing_AccessToken(string accessToken)
        {
            var fixture = new Fixture();
            fixture.Inject(new UriScheme("http"));

            var client = new DfeSignInApiHelper(
                _mockHttpClient.Object)
            {
                AccessToken = accessToken
            };

            Assert.ThrowsAsync<MemberAccessException>(async () => await client.Get<ApiServiceResponse>(fixture.Create<string>()[..5]));
        }

        [TestCase("")]
        [TestCase(null)]
        public void Get_ThrowsException_Missing_Endpoint(string endpoint)
        {
            var fixture = new Fixture();
            fixture.Inject(new UriScheme("http"));

            var client = new DfeSignInApiHelper(
                _mockHttpClient.Object)
            {
                AccessToken = fixture.Create<string>()[..5],
            };

            Assert.ThrowsAsync<ArgumentNullException>(async () => await client.Get<ApiServiceResponse>(endpoint));
        }

        [Test]
        public async Task Get_Return_Expected_When_HttpResponse_IsValid()
        {
            // ARRANGE
            var fixture = new Fixture();
            var userId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var orgId = Guid.NewGuid();

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

            var subjectUnderTest = new DfeSignInApiHelper(httpClient)
            {
                AccessToken = fixture.Create<string>()[..5]
            };

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
                        && req.RequestUri == expectedUri // to this uri
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }


        [Test]
        public async Task Get_Return_Default_When_HttpResponse_IsInValid()
        {
            // ARRANGE
            var fixture = new Fixture();
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

            var subjectUnderTest = new DfeSignInApiHelper(httpClient)
            {
                AccessToken = fixture.Create<string>()[..5]
            };

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
