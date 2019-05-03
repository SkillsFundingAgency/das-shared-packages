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
            throw new NotImplementedException();
        }
    }
}