using Moq;
using NUnit.Framework;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using System;

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
        public void GetAlgorithm_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var jsonWebAlgorithm = CreateJsonWebAlgorithm();
            string algorithm = "HMACSHA512";

            // Act
            var result = new JsonWebAlgorithm().GetAlgorithm(algorithm);

            // Assert
            _mockRepository.VerifyAll();
            Assert.That(result, Is.Not.Null);
        }
    }
}
