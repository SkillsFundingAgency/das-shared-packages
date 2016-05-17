using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public interface IMessageSubSystem
    {
        Task EnqueueAsync(string message);
        Task<SubSystemMessage> DequeueAsync();
    }
}
