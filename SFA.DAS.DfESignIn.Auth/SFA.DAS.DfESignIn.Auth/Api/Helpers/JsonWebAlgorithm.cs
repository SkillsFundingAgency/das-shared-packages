
using System.Collections.Generic;

namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    /// <summary>
    /// Currently supports variants of HMAC Algorithm 
    /// </summary>
    public class JsonWebAlgorithm : IJsonWebAlgorithm
    {
        public readonly Dictionary<string, string> Algorithm;

        public JsonWebAlgorithm()
        {
            Algorithm = new Dictionary<string, string>();

            Algorithm.Add("HMACSHA1", "HS1");
            Algorithm.Add("HMACSHA256", "HS256");
            Algorithm.Add("HMACSHA384", "HS384");
            Algorithm.Add("HMACSHA512", "HS512");
        }

        public string GetAlgorithm(string algorithm)
        {
            if (!Algorithm.ContainsKey(algorithm)) throw new KeyNotFoundException("Cannot find equivalent JSON Web Algorithm");
            return Algorithm[algorithm];
        }
    }

    public interface IJsonWebAlgorithm
    {
        string GetAlgorithm(string algorithm);
    }
}
