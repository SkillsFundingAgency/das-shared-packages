using System;
using System.Linq;
using AutoFixture.NUnit3;
using HashidsNet;
using NUnit.Framework;

namespace SFA.DAS.Encoding.UnitTests.GivenAnEncodingService
{
    [TestFixture]
    public class WhenCallingTryDecode
    {
        [Test, AutoData]
        public void And_Encoded_Value_Is_Valid_Then_Sets_Result_To_Decoded_Value_And_Returns_True(EncodingType encodingType, long expectedResult)
        {
            //Arrange
            var config = EncodingConfigHelper.GenerateRandomConfig();
            var encodingTypeConfig = config.Encodings.Single(x => x.EncodingType == encodingType.ToString());
            var hashids = new Hashids(encodingTypeConfig.Salt, encodingTypeConfig.MinHashLength, encodingTypeConfig.Alphabet);
            var valueToDecode = hashids.EncodeLong(expectedResult);

            //Act
            var encodingService = new EncodingService(config);
            var result = encodingService.TryDecode(valueToDecode, encodingType, out var decodedValue);

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(expectedResult, decodedValue);
        }

        [Test, AutoData]
        public void And_Encoded_Value_Is_Invalid_Then_Sets_Result_To_Zero_And_Returns_False(EncodingType encodingType)
        {
            //Arrange
            var config = EncodingConfigHelper.GenerateRandomConfig();
            var encodingTypeConfig = config.Encodings.Single(x => x.EncodingType == encodingType.ToString());
            var hashids = new Hashids(encodingTypeConfig.Salt, encodingTypeConfig.MinHashLength, encodingTypeConfig.Alphabet);
            var valueToDecode = "THIS_IS_NOT_A_VALID_ENCODED_VALUE";

            //Act
            var encodingService = new EncodingService(config);
            var result = encodingService.TryDecode(valueToDecode, encodingType, out var decodedValue);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, decodedValue);
        }

        [Test, AutoData]
        public void And_Encoded_Value_Is_Empty_String_Then_Throws_Exception(
            EncodingConfig config)
        {
            var encodingType = EncodingType.AccountId;
            var encodingService = new EncodingService(config);

            Assert.Throws<ArgumentException>(() => encodingService.TryDecode("", encodingType, out var decodedValue));
        }
    }
}