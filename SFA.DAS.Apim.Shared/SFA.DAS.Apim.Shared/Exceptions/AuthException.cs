using System;

namespace SFA.DAS.Apim.Shared.Exceptions
{
    public class AuthException : Exception
    {
        public AuthException(string message) : base(message)
        {
            
        }
    }
}
