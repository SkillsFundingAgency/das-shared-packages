using System;

namespace SFA.DAS.Messaging
{
    public class MessageReceivedEventArgs<T> : EventArgs
    {
        public MessageReceivedEventArgs()
        {
        }
        public MessageReceivedEventArgs(T message)
        {
            Message = message;
        }

        public virtual T Message { get; protected set; }
        public bool Handled { get; set; }
    }
}
