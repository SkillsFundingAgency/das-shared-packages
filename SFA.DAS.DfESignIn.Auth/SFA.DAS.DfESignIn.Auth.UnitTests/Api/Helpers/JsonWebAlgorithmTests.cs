using SFA.DAS.DfESignIn.Auth.Api.Helpers;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Api.Helpers
{
    public class JsonWebAlgorithmTests
    {
        [TestCase("HMACSHA1","HS1")]
        [TestCase("HMACSHA256","HS256")]
        [TestCase("HMACSHA384","HS384")]
        [TestCase("HMACSHA512","HS512")]
        public void Then_Algorithm_Is_Correctly_Mapped(string algorithm, string expected)
        {
               // Act
            var result = JsonWebAlgorithm.GetAlgorithm(algorithm);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}