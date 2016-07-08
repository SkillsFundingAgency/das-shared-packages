using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public interface IPollingMessageReceiver
    {
        Task<Message<T>> ReceiveAsAsync<T>() where T : new();
    }
}