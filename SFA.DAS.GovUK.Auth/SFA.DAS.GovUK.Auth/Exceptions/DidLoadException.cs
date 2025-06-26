using System;

namespace SFA.DAS.GovUK.Auth.Exceptions
{
    public class DidLoadException : Exception
    {
        public DidLoadException() :
            base()
        {
        }

        public DidLoadException(string? message) :
            base(message)
        {
        }

        public DidLoadException(string? message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
