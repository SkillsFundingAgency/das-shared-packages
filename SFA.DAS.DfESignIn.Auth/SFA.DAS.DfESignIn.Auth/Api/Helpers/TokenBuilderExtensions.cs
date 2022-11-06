
using System;
using System.Text;

namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    public static class TokenBuilderExtensions
    {
        public static TokenBuilder ForAudience(this TokenBuilder tokenBuilder, string audience)
        {
            if (string.IsNullOrWhiteSpace(audience)) throw new Exception();
            tokenBuilder.Audience = audience;

            tokenBuilder.TokenData.Payload.Add("aud", tokenBuilder.Audience);

            return tokenBuilder;
        }

        public static TokenBuilder UseAlgorithm(this TokenBuilder tokenBuilder, string algorithm)
        {
            if (string.IsNullOrWhiteSpace(algorithm)) throw new Exception("Algorithm");
            tokenBuilder.Algorithm = algorithm;

            tokenBuilder.TokenData.Header.Add("alg", tokenBuilder.JsonWebAlgorithm.GetAlgorithm(algorithm));

            return tokenBuilder;
        }

        public static TokenBuilder WithSecretKey(this TokenBuilder tokenBuilder, string secret)
        {
            if (string.IsNullOrWhiteSpace(secret)) throw new Exception();
            tokenBuilder.SecretKey = Encoding.UTF8.GetBytes(secret);

            return tokenBuilder;
        }

        public static TokenBuilder Issuer(this TokenBuilder tokenBuilder, string issuer)
        {
            if (string.IsNullOrWhiteSpace(issuer)) throw new Exception();
            tokenBuilder.Issuer = issuer;

            tokenBuilder.TokenData.Payload.Add("iss", issuer);

            return tokenBuilder;
        }
    }
}
