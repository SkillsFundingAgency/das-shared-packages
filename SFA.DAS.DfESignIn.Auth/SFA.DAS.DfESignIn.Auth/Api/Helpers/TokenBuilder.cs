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
            #region Check Arguments 
            if (string.IsNullOrWhiteSpace(Algorithm)) throw new ArgumentNullException(nameof(Algorithm));
            if (string.IsNullOrWhiteSpace(Issuer)) throw new ArgumentNullException(nameof(Issuer));
            if (string.IsNullOrWhiteSpace(Audience)) throw new ArgumentNullException(nameof(Audience));
            if (SecretKey == null || SecretKey.Length < 1) throw new ArgumentNullException(nameof(SecretKey));
            #endregion

            var headerBytes = Encoding.UTF8.GetBytes(_tokenDataSerializer.Serialize(TokenData.Header));
            var payloadBytes = Encoding.UTF8.GetBytes(_tokenDataSerializer.Serialize(TokenData.Payload));
            var bytesToSign = Encoding.UTF8.GetBytes($"{_tokenEncoder.Base64Encode(headerBytes)}.{_tokenEncoder.Base64Encode(payloadBytes)}");
            var signedBytes = SignToken(SecretKey, bytesToSign);

            return $"{_tokenEncoder.Base64Encode(headerBytes)}.{_tokenEncoder.Base64Encode(payloadBytes)}.{_tokenEncoder.Base64Encode(signedBytes)}";
        }

        private byte[] SignToken(byte[] key, byte[] bytesToSign)
        {
            using (var algorithm = HMAC.Create(Algorithm))
            {
                #region Check Null Exceptions
                if (algorithm is null) throw new ArgumentNullException(nameof(algorithm));
                #endregion

                algorithm.Key = key;
                return algorithm.ComputeHash(bytesToSign);
            }
        }
    }
}
