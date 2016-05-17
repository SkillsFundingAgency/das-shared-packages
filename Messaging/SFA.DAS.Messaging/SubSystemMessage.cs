using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public abstract class SubSystemMessage
    {
        public virtual string Content { get; protected set; }

        public virtual Task Complete()
        {
            return Task.FromResult<object>(null);
        }
        public virtual Task Abort()
        {
            return Task.FromResult<object>(null);
        }
    }
}
