using Newtonsoft.Json;
using NUnit.Framework;

namespace SFA.DAS.Encoding.UnitTests.GivenAnEncodingConfig
{
    [TestFixture]
    public class WhenDeserialisingFromJson
    {
        private string _json = @"
{
  'Encodings':[
    {
      'EncodingType':'PublicAccountId',
      'Salt':'Salta8c7ae31-fbdd-416d-bfd6-6d137576d5d5',
      'MinHashLength': 234,
      'Alphabet':'Alphabet02411ad6-9da1-452e-a373-9675a446e0f7'
    },{
      'EncodingType':'PublicAccountLegalEntityId',
      'Salt':'Saltfb789382-064a-4f01-9ff0-b21f3643e04a',
      'MinHashLength': 5,
      'Alphabet':'Alphabet69979ed2-4d14-4baa-a587-ce9a5d382ba8'
    },
    {
      'EncodingType': 'AgreementId',
      'Salt': 'Salt6C04203D-1AAE-4593-821F-4F57B02EBF59',
      'MinHashLength': 7,
      'Alphabet': 'Alphabet13BF041F-6B3F-42B4-AF3C-82E292BAB48E'
    }
]}";

        private EncodingConfig _encodingConfig;

        [SetUp]
        public void Setup()
        {
            _encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(_json);
        }

        [Test]
        public void Then_Deserialises_Encodings()
        {
            Assert.IsNotNull(_encodingConfig.Encodings);
        }

        [TestCase(EncodingType.PublicAccountId, 0)]
        [TestCase(EncodingType.PublicAccountLegalEntityId, 1)]
        [TestCase(EncodingType.AgreementId, 2)]
        public void Then_Deserialises_EncodingType(EncodingType encodingType, int index)
        {
            Assert.AreEqual(encodingType, _encodingConfig.Encodings[index].EncodingType);
        }

        [TestCase("Salta8c7ae31-fbdd-416d-bfd6-6d137576d5d5", 0)]
        [TestCase("Saltfb789382-064a-4f01-9ff0-b21f3643e04a", 1)]
        [TestCase("Salt6C04203D-1AAE-4593-821F-4F57B02EBF59", 2)]
        public void Then_Deserialises_Salt(string salt, int index)
        {
            Assert.AreEqual(salt, _encodingConfig.Encodings[index].Salt);
        }

        [TestCase(234, 0)]
        [TestCase(5, 1)]
        [TestCase(7, 2)]
        public void Then_Deserialises_MinHashLength(int minHashLength, int index)
        {
            Assert.AreEqual(minHashLength, _encodingConfig.Encodings[index].MinHashLength);
        }

        [TestCase("Alphabet02411ad6-9da1-452e-a373-9675a446e0f7", 0)]
        [TestCase("Alphabet69979ed2-4d14-4baa-a587-ce9a5d382ba8", 1)]
        [TestCase("Alphabet13BF041F-6B3F-42B4-AF3C-82E292BAB48E", 2)]
        public void Then_Deserialises_Alphabet(string alphabet, int index)
        {
            Assert.AreEqual(alphabet, _encodingConfig.Encodings[index].Alphabet);
        }
    }
}