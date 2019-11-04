using System;
using System.Linq;
using AutoFixture.NUnit3;
using HashidsNet;
using NUnit.Framework;

namespace SFA.DAS.Encoding.UnitTests.GivenAnEncodingService
{
    [TestFixture]
    public class WhenCallingDecode
    {
        [Test, AutoData]
        public void Then_Decodes_Based_On_EncodingType(EncodingType encodingType, long expectedResult)
        {
            //Arrange
            var config = EncodingConfigHelper.GenerateRandomConfig();
            var encodingTypeConfig = config.Encodings.Single(x => x.EncodingType == encodingType.ToString());
            var hashids = new Hashids(encodingTypeConfig.Salt, encodingTypeConfig.MinHashLength, encodingTypeConfig.Alphabet);
            var valueToDecode = hashids.EncodeLong(expectedResult);

            //Act
            var encodingService = new EncodingService(config);
            var result = encodingService.Decode(valueToDecode, encodingType);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test, AutoData]
        public void And_Encoded_Value_Is_Empty_String_Then_Throws_Exception(EncodingType encodingType, long expectedResult)
        {
            //Arrange
            var config = EncodingConfigHelper.GenerateRandomConfig();
            var encodingTypeConfig = config.Encodings.Single(x => x.EncodingType == encodingType.ToString());
            var hashids = new Hashids(encodingTypeConfig.Salt, encodingTypeConfig.MinHashLength, encodingTypeConfig.Alphabet);

            //Act
            var encodingService = new EncodingService(config);

            //Assert
            Assert.Throws<ArgumentException>(() => encodingService.Decode("", encodingType));
        }
    }
}