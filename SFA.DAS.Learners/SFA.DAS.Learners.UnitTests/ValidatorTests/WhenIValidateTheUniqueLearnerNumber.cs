using NUnit.Framework;
using SFA.DAS.Learners.Validators;

namespace SFA.DAS.Learners.UnitTests.ValidatorTests
{
    public class WhenIValidateTheUniqueLearnerNumber
    {
        private UlnValidator _ulnValidator;

        [SetUp]
        public void Arrange()
        {
            _ulnValidator = new UlnValidator();
        }

        [TestCase(1748529632, true)]
        [TestCase(12345678901, false)]
        [TestCase(123456789, false)]
        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(-1234567890, false)]
        public void ThenTheNumberIsNotValidIfItIsNotTenDigitsLong(long uln, bool expectedResult)
        {
            //Act
            var actual = _ulnValidator.Validate(uln);

            //Assert
            Assert.AreEqual(expectedResult,actual);
        }

        [TestCase(1748529632,true)]
        [TestCase(1748529631,false)]
        [TestCase(1748529633,false)]
        public void ThenThenNumberIsValidIfTheSumOfValuesByWeightingLeavesARemainderOfTen(long uln,bool expectedResult)
        {
            //Act
            var actual = _ulnValidator.Validate(uln);

            //Assert
            Assert.AreEqual(expectedResult, actual);
        }


    }
}
