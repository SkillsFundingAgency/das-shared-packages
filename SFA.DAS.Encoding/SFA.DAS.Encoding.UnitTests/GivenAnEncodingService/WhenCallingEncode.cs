using AutoFixture.NUnit3;
using HashidsNet;
using NUnit.Framework;

namespace SFA.DAS.Encoding.UnitTests.GivenAnEncodingService
{
    [TestFixture]
    public class WhenCallingEncode
    {
        [Test, AutoData]
        public void Then_Encodes_Based_On_EncodingType(
            long valueToEncode,
            EncodingConfig config)
        {
            var encodingType = config.Encodings[1].EncodingType;
            var hashids = new Hashids(config.Encodings[1].Salt, config.Encodings[1].MinHashLength, config.Encodings[1].Alphabet);
            var expectedEncoded = hashids.EncodeLong(valueToEncode);
            var encodingService = new EncodingService(config);

            var encodedValue = encodingService.Encode(valueToEncode, encodingType);

            Assert.AreEqual(expectedEncoded,encodedValue);
        }
    }
}