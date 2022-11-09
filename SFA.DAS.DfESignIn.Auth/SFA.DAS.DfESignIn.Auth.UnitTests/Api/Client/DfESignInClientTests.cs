using Moq;
using SFA.DAS.DfESignIn.Auth.Api.Client;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Client
{
    [TestFixture]
    public class DfESignInClientTests
    {
        private MockRepository _mockRepository;

        private Mock<HttpClient> _mockHttpClient;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _mockHttpClient = _mockRepository.Create<HttpClient>();
        }

        private DfESignInClient CreateDfESignInClient()
        {
            return new DfESignInClient(
                _mockHttpClient.Object);
        }

        [Test]
        public void TestDfeSignInClient()
        {
            // Arrange
            var dfESignInClient = CreateDfESignInClient();

            // Assert
            _mockRepository.VerifyAll();
            Assert.That(dfESignInClient, Is.Not.Null);

        }
    }
}
