namespace SFA.DAS.HashingService
{
    using HashidsNet;
    using System;

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

        public long DecodeValue(string id)
        {
            return _hashIds.DecodeLong(id)[0];
        }
    }
}
