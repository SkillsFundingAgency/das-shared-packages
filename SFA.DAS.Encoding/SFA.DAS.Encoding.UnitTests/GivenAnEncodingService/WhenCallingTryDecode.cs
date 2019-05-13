using AutoFixture.NUnit3;
using HashidsNet;
using NUnit.Framework;

namespace SFA.DAS.Encoding.UnitTests.GivenAnEncodingService
{
    [TestFixture]
    public class WhenCallingTryDecode
    {
        [Test, AutoData]
        public void Then_Decodes_Based_On_EncodingType(
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
    }
}