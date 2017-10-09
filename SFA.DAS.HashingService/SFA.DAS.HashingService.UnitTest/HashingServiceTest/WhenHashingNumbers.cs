using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;

namespace SFA.DAS.HashingService.UnitTest.HashingServiceTest
{
    [TestFixture]
    public class WhenHashingNumbers
    {
        private const string Hashstring = "TEST: Dummy hash code London is a city in UK";
        private const string AllowedCharacters = "12345QWERTYUIOPNDGHAK";

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
        public void When_Decode_HashId_IsNullOrEmpty_Should_ThrowException(string valueToDecode)
        {
            // Arrange 
            var _sut = new HashingService(AllowedCharacters, Hashstring);

            //Act
            Action testDelegate = () => _sut.DecodeValue(valueToDecode);

            //Assert
            testDelegate.ShouldThrow<ArgumentException>();
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


        [TestCase("ABCDERT")]
        [TestCase("E12345")]
        [TestCase("ZZZ464")]
        [TestCase("1WQ@")]
        [TestCase("A|?.<>")]
        [TestCase("Z")]
        public void Then_Invalid_Decode_HashId_Should_ThrowException(string hashId)
        {
            // Arrange 
            var _sut = new HashingService(AllowedCharacters, Hashstring);

            //Act
            Action testDelegate = () => _sut.DecodeValue(hashId);

            //Assert
            testDelegate.ShouldThrow<IndexOutOfRangeException>();
        }

    }
}
