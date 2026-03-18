using System;

namespace SFA.DAS.SharedOuterApi.Exceptions
{
    public class AuthException : Exception
    {
        public AuthException(string message) : base(message)
        {
            
        }
    }
}
