using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.DfESignIn.Auth.Api.Client;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Client
{
    [TestFixture]
    public class DfESignInClientFactoryTests
    {
        private MockRepository _mockRepository;

        private Mock<IConfiguration> _mockConfiguration;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _mockConfiguration = _mockRepository.Create<IConfiguration>();

        }

        [Test]
        public void CreateDfESignInClient_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var factory = new DfESignInClientFactory(
                _mockConfiguration.Object);

            string userId = null;
            string organizationId = null;

            var mockIConfigurationSection = new Mock<IConfigurationSection>();
            mockIConfigurationSection.Setup(x => x.GetSection("DfEOidcConfiguration")).Returns(mockIConfigurationSection.Object);

            // Assert
            _mockRepository.VerifyAll();
            Assert.That(factory, Is.Not.Null);
        }
    }
}
