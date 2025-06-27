using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.GovUK.Auth.Exceptions
{
    [ExcludeFromCodeCoverage]
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
