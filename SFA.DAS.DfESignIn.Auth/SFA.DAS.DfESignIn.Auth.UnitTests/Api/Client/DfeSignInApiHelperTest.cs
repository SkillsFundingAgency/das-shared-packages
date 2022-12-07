using AutoFixture;
using Moq;
using SFA.DAS.DfESignIn.Auth.Api.Client;
using SFA.DAS.DfESignIn.Auth.Api.Models;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Client
{
    [TestFixture]
    public class DfeSignInApiHelperTest
    {
        private MockRepository _mockRepository;
        private Mock<HttpClient> _mockHttpClient;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockHttpClient = _mockRepository.Create<HttpClient>();
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
    }
}
