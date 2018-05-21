using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Interfaces
{
    public interface IMessage<out T>
    {
        T Content { get; }
        string Id { get; }
        Task CompleteAsync();
        Task AbortAsync();
    }
}
