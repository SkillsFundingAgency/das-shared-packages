using System;
using System.Linq;
using HashidsNet;

namespace SFA.DAS.Encoding
{
    public class EncodingService : IEncodingService
    {
        private readonly EncodingConfig _config;

        public EncodingService(EncodingConfig config)
        {
            _config = config;
        }

        public string Encode(long value, EncodingType encodingType)
        {
            var encoding = _config.Encodings.Single(enc => enc.EncodingType == encodingType);
            var hashids = new Hashids(encoding.Salt, encoding.MinHashLength, encoding.Alphabet);
            return hashids.EncodeLong(value);
        }

        public long Decode(string value, EncodingType encodingType)
        {
            ValidateInput(value);

            var encoding = _config.Encodings.Single(enc => enc.EncodingType == encodingType);
            var hashids = new Hashids(encoding.Salt, encoding.MinHashLength, encoding.Alphabet);
            return hashids.DecodeLong(value)[0];
        }

        public bool TryDecode(string encodedValue, EncodingType encodingType, out long decodedValue)
        {
            ValidateInput(encodedValue);
            
            var encoding = _config.Encodings.Single(enc => enc.EncodingType == encodingType);
            var hashids = new Hashids(encoding.Salt, encoding.MinHashLength, encoding.Alphabet);
            var decodedValues = hashids.DecodeLong(encodedValue);
            var isValid = decodedValues.Any();

            decodedValue = isValid ? decodedValues[0] : default;

            return isValid;
        }

        private void ValidateInput(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Invalid encoded value", nameof(value));
        }
    }
}