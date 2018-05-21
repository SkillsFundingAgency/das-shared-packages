using System;

namespace SFA.DAS.Messaging
{
    public class MessageContext
    {
        public string MessageId { get; set; }
        public object MessageContent { get; set; }
    }
}