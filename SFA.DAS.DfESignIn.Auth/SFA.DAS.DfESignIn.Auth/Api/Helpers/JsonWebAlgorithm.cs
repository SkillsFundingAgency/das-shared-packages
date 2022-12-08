using System.Collections.Generic;

namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    /// <summary>
    /// Currently supports variants of HMAC Algorithm 
    /// </summary>
    public class JsonWebAlgorithm : IJsonWebAlgorithm
    {
        private readonly Dictionary<string, string> _algorithm;

        public JsonWebAlgorithm()
        {
            _algorithm = new Dictionary<string, string>
            {
                {"HMACSHA1", "HS1"},
                {"HMACSHA256", "HS256"},
                {"HMACSHA384", "HS384"},
                {"HMACSHA512", "HS512"}
            };
        }

        public string GetAlgorithm(string algorithm)
        {
            if (!_algorithm.ContainsKey(algorithm)) throw new KeyNotFoundException("Cannot find equivalent JSON Web Algorithm");
            return _algorithm[algorithm];
        }
    }

    public interface IJsonWebAlgorithm
    {
        string GetAlgorithm(string algorithm);
    }
}
