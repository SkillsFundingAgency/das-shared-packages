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

        [Test]
        public void Then_Deserialises_EncodingType()
        {
            Assert.AreEqual(EncodingType.PublicAccountId, _encodingConfig.Encodings[0].EncodingType);
            Assert.AreEqual(EncodingType.PublicAccountLegalEntityId, _encodingConfig.Encodings[1].EncodingType);
        }

        [Test]
        public void Then_Deserialises_Salt()
        {
            Assert.AreEqual("Salta8c7ae31-fbdd-416d-bfd6-6d137576d5d5", _encodingConfig.Encodings[0].Salt);
            Assert.AreEqual("Saltfb789382-064a-4f01-9ff0-b21f3643e04a", _encodingConfig.Encodings[1].Salt);
        }

        [Test]
        public void Then_Deserialises_MinHashLength()
        {
            Assert.AreEqual(234, _encodingConfig.Encodings[0].MinHashLength);
            Assert.AreEqual(5, _encodingConfig.Encodings[1].MinHashLength);
        }

        [Test]
        public void Then_Deserialises_Alphabet()
        {
            Assert.AreEqual("Alphabet02411ad6-9da1-452e-a373-9675a446e0f7", _encodingConfig.Encodings[0].Alphabet);
            Assert.AreEqual("Alphabet69979ed2-4d14-4baa-a587-ce9a5d382ba8", _encodingConfig.Encodings[1].Alphabet);
        }
    }
}