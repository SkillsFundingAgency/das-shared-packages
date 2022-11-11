using System;
using System.Security.Cryptography;
using System.Text;

namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    public class TokenBuilder
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public byte[] SecretKey { get; set; }
        public string Algorithm { get; set; }
        public ITokenData TokenData { get; set; }
        public readonly IJsonWebAlgorithm JsonWebAlgorithm;
        private readonly ITokenEncoder _tokenEncoder;
        private readonly ITokenDataSerializer _tokenDataSerializer;


        public TokenBuilder(ITokenDataSerializer tokenDataSerializer, ITokenData tokenData, ITokenEncoder tokenEncoder, IJsonWebAlgorithm jsonWebAlgorithm)
        {
            _tokenDataSerializer = tokenDataSerializer;
            TokenData = tokenData;
            _tokenEncoder = tokenEncoder;
            JsonWebAlgorithm = jsonWebAlgorithm;
        }

        public string CreateToken()
        {
            if (string.IsNullOrWhiteSpace(Algorithm)) throw new Exception("Algorithm");
            if (string.IsNullOrWhiteSpace(Issuer)) throw new Exception("Issuer");
            if (string.IsNullOrWhiteSpace(Audience)) throw new Exception("Audience");
            if (SecretKey == null || SecretKey.Length < 1) throw new Exception("SecretKey");

            byte[] headerBytes = Encoding.UTF8.GetBytes(_tokenDataSerializer.Serialize(TokenData.Header));
            byte[] payloadBytes = Encoding.UTF8.GetBytes(_tokenDataSerializer.Serialize(TokenData.Payload));
            byte[] bytesToSign = Encoding.UTF8.GetBytes($"{_tokenEncoder.Base64Encode(headerBytes)}.{_tokenEncoder.Base64Encode(payloadBytes)}");
            byte[] signedBytes = SignToken(SecretKey, bytesToSign);

            string token = $"{_tokenEncoder.Base64Encode(headerBytes)}.{_tokenEncoder.Base64Encode(payloadBytes)}.{_tokenEncoder.Base64Encode(signedBytes)}";

            return token;
        }

        private byte[] SignToken(byte[] key, byte[] bytesToSign)
        {
            using (var algorithm = HMAC.Create(Algorithm))
            {
                if (algorithm == null)
                {
                    throw new Exception("Crytography Creation");
                }
                algorithm.Key = key;
                return algorithm.ComputeHash(bytesToSign);
            }
        }
    }
}
