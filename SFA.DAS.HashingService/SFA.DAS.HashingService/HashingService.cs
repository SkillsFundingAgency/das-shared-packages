namespace SFA.DAS.HashingService
{
    using HashidsNet;
    using System;

    public class HashingService : IHashingService
    {
        private readonly Hashids _hashIds;
      
        public HashingService(string allowedCharacters, string hashstring)
        {
            if (string.IsNullOrEmpty(allowedCharacters) || string.IsNullOrEmpty(hashstring))
            {
                throw new ArgumentException("Hash String or Allowed Characters cannot be null or empty");
            }
            
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
