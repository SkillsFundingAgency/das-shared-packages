using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;

namespace SFA.DAS.Encoding.UnitTests.GivenAnEncodingService
{
    public static class EncodingConfigHelper
    {
        public static EncodingConfig GenerateRandomConfig()
        {
            var result = new EncodingConfig
            {
                Encodings = new List<Encoding>()
            };

            var fixture = new Fixture();

            var encodingTypes = Enum.GetValues(typeof(EncodingType)).Cast<EncodingType>();

            foreach (var encoding in encodingTypes)
            {
                result.Encodings.Add(new Encoding
                {
                    Alphabet = "Alphabet" + fixture.Create<string>(),
                    EncodingType = encoding.ToString(),
                    MinHashLength = fixture.Create<int>(),
                    Salt = fixture.Create<string>()
                });
            }

            return result;
        }
    }
}
