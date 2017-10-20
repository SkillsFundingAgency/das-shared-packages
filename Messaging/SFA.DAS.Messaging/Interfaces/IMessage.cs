using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Interfaces
{
    public interface IMessage<out T>
    {
        T Content { get; }
        Task CompleteAsync();
        Task AbortAsync();
    }
}
