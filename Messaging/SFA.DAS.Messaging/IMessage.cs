using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public interface IMessage<T>
    {
        T Content { get; }
        Task CompleteAsync();
        Task AbortAsync();
    }
}
