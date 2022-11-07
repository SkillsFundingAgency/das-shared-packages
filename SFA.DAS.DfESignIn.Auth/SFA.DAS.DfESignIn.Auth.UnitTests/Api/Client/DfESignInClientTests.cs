using Moq;
using SFA.DAS.DfESignIn.Auth.Api.Client;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Client
{
    [TestFixture]
    public class DfESignInClientTests
    {
        private MockRepository mockRepository;

        private Mock<HttpClient> mockHttpClient;

        [SetUp]
        public void SetUp()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockHttpClient = this.mockRepository.Create<HttpClient>();
        }

        private DfESignInClient CreateDfESignInClient()
        {
            return new DfESignInClient(
                this.mockHttpClient.Object);
        }

        [Test]
        public void TestDfeSignInClient()
        {
            // Arrange
            var dfESignInClient = this.CreateDfESignInClient();

            // Assert
            this.mockRepository.VerifyAll();
        }
    }
}
