using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Exceptions
{
    public class MessageContextException : Exception
    {
        public MessageContextException(string message) : base(message)
        {
            // just call base
        }

        public MessageContextException() : base()
        {
            // just call base
        }

        public MessageContextException(Type messageContentType, string message) : base($"Message type: [{messageContentType?.FullName}] {message}")
        {
            // just call base
        }
    }
}
