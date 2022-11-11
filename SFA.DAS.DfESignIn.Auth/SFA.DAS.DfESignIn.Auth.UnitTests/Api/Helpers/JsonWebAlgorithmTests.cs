using AutoFixture;
using Moq;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Helpers
{
    [TestFixture]
    public class JsonWebAlgorithmTests
    {
        private MockRepository _mockRepository;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
        }

        [Test]
        public void GetAlgorithm_TypeExpectedResult_HS1()
        {
            // Arrange
            string algorithm = "HMACSHA1";

            // Act
            var result = new JsonWebAlgorithm().GetAlgorithm(algorithm);

            // Assert
            Assert.That(result, Is.EqualTo("HS1"));
        }

        [Test]
        public void GetAlgorithm_TypeExpectedResult_HS256()
        {
            // Arrange
            string algorithm = "HMACSHA256";

            // Act
            var result = new JsonWebAlgorithm().GetAlgorithm(algorithm);

            // Assert
            Assert.That(result, Is.EqualTo("HS256"));
        }

        [Test]
        public void GetAlgorithm_TypeExpectedResult_HMACSHA384()
        {
            // Arrange
            string algorithm = "HMACSHA384";

            // Act
            var result = new JsonWebAlgorithm().GetAlgorithm(algorithm);

            // Assert
            Assert.That(result, Is.EqualTo("HS384"));
        }

        [Test]
        public void GetAlgorithm_TypeExpectedResult_HMACSHA512()
        {
            // Arrange
            string algorithm = "HMACSHA512";

            // Act
            var result = new JsonWebAlgorithm().GetAlgorithm(algorithm);

            // Assert
            Assert.That(result, Is.EqualTo("HS512"));
        }
    }
}