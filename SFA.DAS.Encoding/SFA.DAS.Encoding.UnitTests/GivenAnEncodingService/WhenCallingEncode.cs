using System.Linq;
using AutoFixture.NUnit3;
using HashidsNet;
using NUnit.Framework;

namespace SFA.DAS.Encoding.UnitTests.GivenAnEncodingService
{
    [TestFixture]
    public class WhenCallingEncode
    {
        [Test, AutoData]
        public void Then_Encodes_Based_On_EncodingType(EncodingType encodingType, long valueToEncode)
        {
            //Arrange
            var config = EncodingConfigHelper.GenerateRandomConfig();
            var encodingTypeConfig = config.Encodings.Single(x => x.EncodingType == encodingType.ToString());
            var hashids = new Hashids(encodingTypeConfig.Salt, encodingTypeConfig.MinHashLength, encodingTypeConfig.Alphabet);
            var expectedEncoded = hashids.EncodeLong(valueToEncode);

            //Act
            var encodingService = new EncodingService(config);
            var result = encodingService.Encode(valueToEncode, encodingType);

            //Assert
            Assert.AreEqual(expectedEncoded,result);
        }
    }
}