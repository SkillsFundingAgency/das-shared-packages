namespace SFA.DAS.HashingService
{
    using HashidsNet;
    using System;
    using System.Linq;

    public class HashingService : IHashingService
    {
        private readonly Hashids _hashIds;
      
        public HashingService(string allowedCharacters, string hashstring)
        {
            if (string.IsNullOrEmpty(allowedCharacters))
                throw new ArgumentException("Value cannot be null", nameof(allowedCharacters));

            if (string.IsNullOrEmpty(hashstring))
                throw new ArgumentException("Value cannot be null", nameof(hashstring));

            _hashIds = new Hashids(hashstring, 6, allowedCharacters);
        }

        public string HashValue(long id)
        {
            return _hashIds.EncodeLong(id);
        }

        public string HashValue(Guid id)
        {
            return _hashIds.Encode(id.ToByteArray().Select(Convert.ToInt32).ToArray());
        }
        
        public long DecodeValue(string id)
        {

            if (string.IsNullOrEmpty(id?.Trim()))
                throw new ArgumentException("Invalid hash Id", nameof(id));

            return _hashIds.DecodeLong(id)[0];
        }

        public Guid DecodeValueToGuid(string id)
        {
            return new Guid(_hashIds.Decode(id).Select(Convert.ToByte).ToArray());
        }


    }
}
