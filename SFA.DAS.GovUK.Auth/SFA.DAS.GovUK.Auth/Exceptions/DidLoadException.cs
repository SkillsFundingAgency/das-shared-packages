using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.GovUK.Auth.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class DidLoadException : Exception
    {
        public DidLoadException() :
            base()
        {
        }

        public DidLoadException(string message) :
            base(message)
        {
        }

        public DidLoadException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
