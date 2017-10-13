using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;

namespace SFA.DAS.HashingService.UnitTest.HashingServiceTest
{
    [TestFixture]
    public class WhenHashingIds
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
        public void Then_Numeric_HashValue_Should_Equal_DecodeValue(long expectTedValue)
        {
            // Arrange 
            var _sut = new HashingService(AllowedCharacters, Hashstring);

            //Act
            var hash = _sut.HashValue(expectTedValue);
            var actualValue = _sut.DecodeValue(hash);

            //Assert
            expectTedValue.Should().Be(actualValue);
        }

        [TestCase("b262ecd2-7512-4e7a-abe2-2f880a7d5294")]
        [TestCase("{3A5CBD18-2FF4-4A94-850F-88D1FB5B9048}")]
        [TestCase("{0039a928-33ea-492e-a798-79df52f941cf}")]
        [TestCase("947127c479c44c2e85d3ea1a8bf24bdb")]
        [TestCase("ddb8bb604d0b4f529f2a7b7833762d2f")]
        public void Then_Guid_HashValue_Should_Equal_DecodeValue(string hashValue)
        {
            // Arrange 
            var _sut = new HashingService(AllowedCharacters, Hashstring);

            //Act
            Guid expectedValue = Guid.Parse(hashValue);

            var hash = _sut.HashValue(expectedValue);
            var actualValue = _sut.DecodeValueToGuid(hash);

            //Assert
            expectedValue.Should().Be(actualValue);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        [TestCase("ABCDERT")]
        [TestCase("E12345")]
        [TestCase("ZZZ464")]
        [TestCase("1WQ@")]
        [TestCase("A|?.<>")]
        [TestCase("Z")]
        public void When_DecodingToGuid_Invalid_HashId_Should_ThrowException(string valueToDecode)
        {
            // Arrange 
            var _sut = new HashingService(AllowedCharacters, Hashstring);

            //Act
            Action testDelegate = () => _sut.DecodeValueToGuid(valueToDecode);

            //Assert
            testDelegate.ShouldThrow<ArgumentException>();
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
