using System;

namespace SFA.DAS.SharedOuterApi.Exceptions
{
    public class NotFoundException<T> : Exception
    {
        public NotFoundException() 
            : base($"[{typeof(T).Name}] cannot be found")
        {
            
        }

        public NotFoundException(string message) 
            : base(message)
        {
            
        }

        public NotFoundException(Exception innerException) 
            : base($"[{typeof(T).Name}] cannot be found", innerException)
        {
            
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
    }
}