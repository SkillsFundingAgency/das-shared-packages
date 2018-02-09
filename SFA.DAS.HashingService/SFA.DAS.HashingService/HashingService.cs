namespace SFA.DAS.HashingService
{
    using HashidsNet;
    using System;
    using System.Linq;
    using System.Text;

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

        public string HashValue(string id)
        {
            var hexValue = StringToHex(id).Replace("-", "");
            return _hashIds.EncodeHex(hexValue);
        }

        public long DecodeValue(string id)
        {
            ValidateInput(id);
            return _hashIds.DecodeLong(id)[0];
        }

        public Guid DecodeValueToGuid(string id)
        {
            return new Guid(_hashIds.Decode(id).Select(Convert.ToByte).ToArray());
        }

        public string DecodeValueToString(string id)
        {
            ValidateInput(id);
            var hexValue = _hashIds.DecodeHex(id);
            var actualId = FromHexToString(hexValue);

            return actualId;
        }

        private string FromHexToString(string hex)
        {
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return Encoding.ASCII.GetString(raw);
        }

        private string StringToHex(string stringValue)
        {
            byte[] ba = Encoding.Default.GetBytes(stringValue);
            var hexString = BitConverter.ToString(ba);
            return hexString;
        }

        private void ValidateInput(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid hash Id", nameof(id));
        }
    }
}
