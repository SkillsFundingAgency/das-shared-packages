using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.DfESignIn.Auth.Api.Client;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Client
{
    [TestFixture]
    public class DfESignInClientFactoryTests
    {
        private MockRepository mockRepository;

        private Mock<IConfiguration> mockConfiguration;

        [SetUp]
        public void SetUp()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockConfiguration = this.mockRepository.Create<IConfiguration>();

        }

        private DfESignInClientFactory CreateFactory()
        {
            return new DfESignInClientFactory(
                this.mockConfiguration.Object);
        }

        [Test]
        public void CreateDfESignInClient_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var factory = this.CreateFactory();
            string userId = null;
            string organizationId = null;

            var mockIConfigurationSection = new Mock<IConfigurationSection>();
            mockIConfigurationSection.Setup(x => x.GetSection("DfEOidcConfiguration")).Returns(mockIConfigurationSection.Object);

            // Assert
            this.mockRepository.VerifyAll();
            Assert.That(factory, Is.Not.Null);
        }
    }
}
