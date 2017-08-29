using NUnit.Framework;
using SFA.DAS.Learners.Validators;

namespace SFA.DAS.Learners.UnitTests.ValidatorTests
{
    [TestFixture]
    public class WhenIValidateTheUniqueLearnerNumber
    {
        private UlnValidator _ulnValidator;

        [SetUp]
        public void Arrange()
        {
            _ulnValidator = new UlnValidator();
        }

        [TestCase("1748529632", UlnValidationResult.Success)]
        [TestCase("12345678901", UlnValidationResult.IsInValidTenDigitUlnNumber)]
        [TestCase("123456789", UlnValidationResult.IsInValidTenDigitUlnNumber)]
        [TestCase("0", UlnValidationResult.IsInValidTenDigitUlnNumber)]
        [TestCase("1", UlnValidationResult.IsInValidTenDigitUlnNumber)]
        public void ThenTheNumberIsNotValidIfItIsNotTenDigitsLong(string uln, UlnValidationResult expectedResult)
        {
            //Act
            var actual = _ulnValidator.Validate(uln);

            //Assert
            Assert.AreEqual(expectedResult,actual);
        }

        [Test]
        public void ThenTheNumberIsNotValidIfIsAnEmptyString()
        {
            //Arrange
            var uln = string.Empty;

            //Act
            var actual = _ulnValidator.Validate(uln);

            //Assert
            Assert.AreEqual(UlnValidationResult.IsEmptyUlnNumber, actual);
        }

        [Test]
        public void ThenTheNumberIsNotValidIfIsAlphaNumeric()
        {
            //Arrange
            var uln = "ABC4567890";

            //Act
            var actual = _ulnValidator.Validate(uln);

            //Assert
            Assert.AreEqual(UlnValidationResult.IsInValidTenDigitUlnNumber, actual);
        }

        [Test]
        public void ThenTheNumberIsNotValidIfIsLessThanZero()
        {
            //Arrange
            var uln = "-174852963";

            //Act
            var actual = _ulnValidator.Validate(uln);

            //Assert
            Assert.AreEqual(UlnValidationResult.IsInValidTenDigitUlnNumber, actual);
        }

        [TestCase("1748529632", UlnValidationResult.Success)]
        [TestCase("1748529631", UlnValidationResult.IsInvalidUln)]
        [TestCase("1748529633", UlnValidationResult.IsInvalidUln)]
        [TestCase("9999999999", UlnValidationResult.IsInvalidUln)]
        public void ThenThenNumberIsValidIfTheSumOfValuesByWeightingLeavesARemainderOfTen(string uln, UlnValidationResult expectedResult)
        {
            //Act
            var actual = _ulnValidator.Validate(uln);

            //Assert
            Assert.AreEqual(expectedResult, actual);
        }


    }
}
