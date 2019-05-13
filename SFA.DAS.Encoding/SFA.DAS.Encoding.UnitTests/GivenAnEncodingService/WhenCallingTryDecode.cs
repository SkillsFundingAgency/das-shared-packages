using System;
using AutoFixture.NUnit3;
using HashidsNet;
using NUnit.Framework;

namespace SFA.DAS.Encoding.UnitTests.GivenAnEncodingService
{
    [TestFixture]
    public class WhenCallingTryDecode
    {
        [Test, AutoData]
        public void And_Encoded_Value_Is_Valid_Then_Sets_Result_To_Decoded_Value_And_Returns_True(
            long expectedDecoded,
            EncodingConfig config)
        {
            var encodingType = config.Encodings[1].EncodingType;
            var hashids = new Hashids(config.Encodings[1].Salt, config.Encodings[1].MinHashLength, config.Encodings[1].Alphabet);
            var encoded = hashids.EncodeLong(expectedDecoded);
            var encodingService = new EncodingService(config);
            var isValid = encodingService.TryDecode(encoded, encodingType, out var decodedValue);
            
            Assert.IsTrue(isValid);
            Assert.AreEqual(expectedDecoded,decodedValue);
        }
        
        [Test, AutoData]
        public void And_Encoded_Value_Is_Invalid_Then_Sets_Result_To_Zero_And_Returns_False(
            long expectedDecoded,
            EncodingConfig config)
        {
            var encodingType = config.Encodings[1].EncodingType;
            var hashids = new Hashids(config.Encodings[1].Salt, config.Encodings[1].MinHashLength, config.Encodings[1].Alphabet);
            var encoded = $"_{hashids.EncodeLong(expectedDecoded)}_";
            var encodingService = new EncodingService(config);
            var isValid = encodingService.TryDecode(encoded, encodingType, out var decodedValue);
            
            Assert.IsFalse(isValid);
            Assert.AreEqual(0,decodedValue);
        }

        [Test, AutoData]
        public void And_Encoded_Value_Is_Empty_String_Then_Throws_Exception(
            EncodingConfig config)
        {
            var encodingType = config.Encodings[1].EncodingType;
            var encodingService = new EncodingService(config);

            Assert.Throws<ArgumentException>(() => encodingService.TryDecode("", encodingType, out var decodedValue));
        }
    }
}