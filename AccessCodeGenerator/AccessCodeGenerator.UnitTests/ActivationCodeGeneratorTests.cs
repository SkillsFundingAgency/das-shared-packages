using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.CodeGenerator;
using SFA.DAS.CodeGenerator.UnitTests;

namespace SFA.DAS.AccessCodeGenerator.UnitTests
{

    [TestFixture]
    public class ActivationCodeGeneratorTests
    {
        [Test]
        public void ShouldGenerateRandomSixCharacterAlphanumericCodes()
        {
            const int sampleSize = 100;
            var sampleCodes = new List<string>();

            for (var i = 0; i < sampleSize; i++)
            {
                // Arrange.
                var generator = new RandomCodeGenerator();

                // Act.
                var code = generator.GenerateAlphaNumeric();

                // Assert.
                Assert.IsFalse(string.IsNullOrWhiteSpace(code));
                Assert.AreEqual(6, code.Length);
                Assert.IsTrue(code.All(c => RandomCodeGenerator.Alphanumerics.Contains(c)));

                CollectionAssert.DoesNotContain(sampleCodes, code);

                sampleCodes.Add(code);
            }

            Assert.AreEqual(sampleSize, sampleCodes.Count);
        }

        [Test]
        public void ShouldGenerateRandomFourCharacterNumericCodes()
        {
            const int sampleSize = 10;
            var sampleCodes = new List<string>();

            for (var i = 0; i < sampleSize; i++)
            {
                // Arrange.
                var generator = new RandomCodeGenerator();

                // Act.
                var code = generator.GenerateNumeric();

                // Assert.
                Assert.IsFalse(string.IsNullOrWhiteSpace(code));
                Assert.AreEqual(4, code.Length);
                Assert.IsTrue(code.All(c => RandomCodeGenerator.Numerics.Contains(c)));

                CollectionAssert.DoesNotContain(sampleCodes, code);

                sampleCodes.Add(code);
            }

            Assert.AreEqual(sampleSize, sampleCodes.Count);
        }

        [Test]
        public void ShouldGenerateCodeOfFirstCharacterInSetWhenUsingRandomNumberGeneratorTestImplementation()
        {
            //Arrange
            var generator = new RandomCodeGenerator(new RandomNumberGeneratorTestClass());

            //Act
            var code = generator.GenerateAlphaNumeric(4);

            //Assert
            Assert.AreEqual("4444",code);
        }
    }
}
