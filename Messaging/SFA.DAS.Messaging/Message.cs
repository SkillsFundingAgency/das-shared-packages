using System;
using System.Threading.Tasks;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging
{
    [Obsolete("This class will be removed in a future release. Base classes should implement IMessage directly.")]
    public abstract class Message<T> : IMessage<T>
    {
        protected Message(T content)
        {
            Content = content;
        }
        protected Message()
        {

        }
        public T Content { get; protected set; }
        public string Id { get; }
        public abstract Task CompleteAsync();
        public abstract Task AbortAsync();
    }
}