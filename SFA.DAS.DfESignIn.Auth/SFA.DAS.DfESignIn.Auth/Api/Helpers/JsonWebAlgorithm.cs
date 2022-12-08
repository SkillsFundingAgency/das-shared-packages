using System.Collections.Generic;

namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    public static class JsonWebAlgorithm
    {
        private static readonly Dictionary<string, string> Algorithm = new Dictionary<string, string>
        {
            {"HMACSHA1", "HS1"},
            {"HMACSHA256", "HS256"},
            {"HMACSHA384", "HS384"},
            {"HMACSHA512", "HS512"}
        };

        public static string GetAlgorithm(string algorithm)
        {
            if (!Algorithm.ContainsKey(algorithm)) throw new KeyNotFoundException("Cannot find equivalent JSON Web Algorithm");
            return Algorithm[algorithm];
        }
    }
}
