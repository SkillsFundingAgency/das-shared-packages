using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public abstract class Message<T>
    {
        protected Message(T content)
        {
            Content = content;
        }

        public T Content { get; }

        public abstract Task CompleteAsync();
        public abstract Task AbortAsync();
    }
}