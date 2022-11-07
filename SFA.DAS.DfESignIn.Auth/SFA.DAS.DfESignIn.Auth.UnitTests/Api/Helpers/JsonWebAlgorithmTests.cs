using Moq;
using NUnit.Framework;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using System;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Helpers
{
    [TestFixture]
    public class JsonWebAlgorithmTests
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
        }

        private JsonWebAlgorithm CreateJsonWebAlgorithm()
        {
            return new JsonWebAlgorithm();
        }

        [Test]
        public void GetAlgorithm_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var jsonWebAlgorithm = this.CreateJsonWebAlgorithm();
            string algorithm = "HMACSHA512";

            // Act
            var result = jsonWebAlgorithm.GetAlgorithm(algorithm);

            // Assert
            this.mockRepository.VerifyAll();
            Assert.That(result, Is.Not.Null);
        }
    }
}
