using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public abstract class SubSystemMessage
    {
        public virtual string Content { get; protected set; }

        public virtual Task CompleteAsync()
        {
            return Task.FromResult<object>(null);
        }
        public virtual Task AbortAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
