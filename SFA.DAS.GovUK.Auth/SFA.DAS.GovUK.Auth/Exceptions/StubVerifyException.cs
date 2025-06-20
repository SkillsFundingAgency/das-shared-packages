using System;

namespace SFA.DAS.GovUK.Auth.Exceptions
{
    public class StubVerifyException : Exception
    {
        public StubVerifyException() :
            base()
        {
        }

        public StubVerifyException(string? message) :
            base(message)
        {
        }

        public StubVerifyException(string? message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
