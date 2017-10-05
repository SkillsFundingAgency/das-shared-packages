using HashidsNet;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.HashingService.UnitTest.HashingServiceTest
{
    [TestFixture]
    public class WhenHashingNumbers
    {
        private const string Hashstring = "SFA: digital apprenticeship service";
        private const string AllowedCharacters = "46789BCDFGHJKLMNPRSTVWXY";

        [Test]
        public void When_AllowedCharacter_IsEmpty_Should_Throw_Exception()
        {
            //Act
            ActualValueDelegate<object> testDelegate = () => new HashingService(string.Empty, Hashstring);

            //Assert
            Assert.That(testDelegate, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void When_Hashstring_IsEmpty_Should_Throw_Exception()
        {
            //Act
            ActualValueDelegate<object> testDelegate = () => new HashingService(AllowedCharacters, string.Empty);

            //Assert
            Assert.That(testDelegate, Throws.TypeOf<ArgumentException>());
        }

        [TestCase(123456)]
        [TestCase(399999)]
        [TestCase(233333)]
        [TestCase(333333)]
        [TestCase(444444)]
        [TestCase(0)]
        public void Then_HashValue_Should_Equal_DecodeValue(long valueToHash)
        {
            // Arrange 
            var _sut = new HashingService(AllowedCharacters, Hashstring);

            //Act
            var hash = _sut.HashValue(valueToHash);
            var actualValue = _sut.DecodeValue(hash);

            //Assert
            Assert.AreEqual(valueToHash, actualValue);
        }

    }
}
