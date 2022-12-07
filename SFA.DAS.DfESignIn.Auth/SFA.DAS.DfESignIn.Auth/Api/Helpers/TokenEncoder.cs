using System;

namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    public class TokenEncoder : ITokenEncoder
    {
        public string Base64Encode(byte[] stringInput)
        {
            if (stringInput == null)
            {
                throw new ArgumentNullException("Encoding Error: stringInput");
            }
            if (stringInput.Length == 0)
            {
                throw new ArgumentOutOfRangeException("Encoding Error: stringInput");
            }
            char[] separator = new char[] { '=' };
            return Convert.ToBase64String(stringInput).Split(separator)[0].Replace('+', '-').Replace('/', '_');
        }
    }

    public interface ITokenEncoder
    {
        string Base64Encode(byte[] stringInput);
    }
}
