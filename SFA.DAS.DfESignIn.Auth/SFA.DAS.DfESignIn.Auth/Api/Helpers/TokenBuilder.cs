using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Constants;

namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    public interface ITokenBuilder
    {
        string CreateToken();
    }
    public class TokenBuilder : ITokenBuilder
    {
        private byte[] SecretKey { get; set; }
        private readonly ITokenDataSerializer _tokenDataSerializer;
        private readonly TokenData _tokenData;
        
        public TokenBuilder(ITokenDataSerializer tokenDataSerializer, IOptions<DfEOidcConfiguration> configuration)
        {
            _tokenDataSerializer = tokenDataSerializer;
            _tokenData = new TokenData();
            _tokenData.Header.Add("typ", AuthConfig.TokenType);
            _tokenData.Header.Add("alg", JsonWebAlgorithm.GetAlgorithm(AuthConfig.Algorithm));
            _tokenData.Payload.Add("aud", AuthConfig.Aud);
            _tokenData.Payload.Add("iss", configuration.Value.ClientId);
            
            SecretKey = Encoding.UTF8.GetBytes(configuration.Value.APIServiceSecret);
        }

        public string CreateToken()
        {   
            var headerBytes = Encoding.UTF8.GetBytes(_tokenDataSerializer.Serialize(_tokenData.Header));
            var payloadBytes = Encoding.UTF8.GetBytes(_tokenDataSerializer.Serialize(_tokenData.Payload));
            var bytesToSign = Encoding.UTF8.GetBytes($"{Base64Encode(headerBytes)}.{Base64Encode(payloadBytes)}");
            var signedBytes = SignToken(SecretKey, bytesToSign);

            return $"{Base64Encode(headerBytes)}.{Base64Encode(payloadBytes)}.{Base64Encode(signedBytes)}";
        }

        private byte[] SignToken(byte[] key, byte[] bytesToSign)
        {
            using (var algorithm = HMAC.Create(AuthConfig.Algorithm))
            {
                algorithm.Key = key;
                return algorithm.ComputeHash(bytesToSign);
            }
        }
        private string Base64Encode(byte[] stringInput)
        {
            return Convert.ToBase64String(stringInput).Split(new[] { '=' })[0].Replace('+', '-').Replace('/', '_');
        }
    }
}

