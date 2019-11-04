using System.Collections.Generic;
using AutoFixture;
using NUnit.Framework;

namespace SFA.DAS.Encoding.UnitTests.GivenAnEncodingService
{
    [TestFixture]
    public class WhenConstructingEncodingService
    {
        [Test]
        public void Unrecognised_EncodingType_Does_Not_Cause_Exception()
        {
            var autoFixture = new Fixture();

            var config = new EncodingConfig
            {
                Encodings = new List<Encoding>
                {
                    new Encoding
                    {
                        Alphabet = autoFixture.Create<string>(),
                        EncodingType = "DOES_NOT_EXIST",
                        MinHashLength = autoFixture.Create<int>(),
                        Salt = autoFixture.Create<string>()
                    }
                }
            };

            Assert.DoesNotThrow(() => new EncodingService(config));
        }
    }
}
