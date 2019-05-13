using System;
using System.Collections.Generic;
using System.Linq;
using HashidsNet;

namespace SFA.DAS.Encoding
{
    public class EncodingService : IEncodingService
    {
        private readonly Dictionary<EncodingType, Hashids> _encodings;

        public EncodingService(EncodingConfig config)
        {
            _encodings = config.Encodings.ToDictionary(e => e.EncodingType, e => new Hashids(e.Salt, e.MinHashLength, e.Alphabet));
        }

        public string Encode(long value, EncodingType encodingType)
        {
            return _encodings[encodingType].EncodeLong(value);
        }

        public long Decode(string value, EncodingType encodingType)
        {
            ValidateInput(value);
            
            return _encodings[encodingType].DecodeLong(value)[0];
        }

        public bool TryDecode(string encodedValue, EncodingType encodingType, out long decodedValue)
        {
            ValidateInput(encodedValue);
            
            var decodedValues = _encodings[encodingType].DecodeLong(encodedValue);
            var isValid = decodedValues.Any();
            
            decodedValue = isValid ? decodedValues[0] : 0;
            
            return isValid;
        }

        private void ValidateInput(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Invalid encoded value", nameof(value));
        }
    }
}