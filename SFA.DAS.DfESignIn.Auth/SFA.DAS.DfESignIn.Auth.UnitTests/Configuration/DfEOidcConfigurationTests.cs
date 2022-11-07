using Moq;
using NUnit.Framework;
using SFA.DAS.DfESignIn.Auth.Configuration;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Configuration
{
    [TestFixture]
    public class DfEOidcConfigurationTests
    {
        private MockRepository mockRepository;



        [SetUp]
        public void SetUp()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
        }

        private DfEOidcConfiguration CreateDfEOidcConfiguration()
        {
            return new DfEOidcConfiguration();
        }

        [Test]
        public void TestConfiguration()
        {
            // Arrange
            var dfEOidcConfiguration = this.CreateDfEOidcConfiguration();

            // Assert
            this.mockRepository.VerifyAll();
            Assert.That(dfEOidcConfiguration, Is.Not.Null);
        }
    }
}
