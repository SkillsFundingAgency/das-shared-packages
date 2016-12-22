using System;

namespace SFA.DAS.OidcMiddleware
{
    public class OidcAuthenticationException : Exception
    {
        public OidcAuthenticationException(string message)
            : base(message)
        {

        }
    }
}
