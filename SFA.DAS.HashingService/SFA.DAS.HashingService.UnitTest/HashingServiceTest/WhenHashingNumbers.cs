using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;

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
            Action testDelegate = () => new HashingService(string.Empty, Hashstring);

            //Assert
            testDelegate.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void When_Hashstring_IsEmpty_Should_Throw_Exception()
        {
            //Act
            Action testDelegate = () => new HashingService(AllowedCharacters, string.Empty);

            //Assert
            testDelegate.ShouldThrow<ArgumentException>();
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Given_Invalid_Decode_Hash_Should_ThrowException(string valueToDecode)
        {
            // Arrange 
            var _sut = new HashingService(AllowedCharacters, Hashstring);

            //Act
            Action testDelegate = () => _sut.DecodeValue(valueToDecode);

            //Assert
            testDelegate.ShouldThrow<IndexOutOfRangeException>();
        }

        [TestCase(123456)]
        [TestCase(399999)]
        [TestCase(233333)]
        [TestCase(333333)]
        [TestCase(444444)]
        [TestCase(0)]
        public void Then_HashValue_Should_Equal_DecodeValue(long expectTedValue)
        {
            // Arrange 
            var _sut = new HashingService(AllowedCharacters, Hashstring);

            //Act
            var hash = _sut.HashValue(expectTedValue);
            var actualValue = _sut.DecodeValue(hash);

            //Assert
           expectTedValue.Should().Be(actualValue);
        }

    }
}
