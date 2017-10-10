using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public interface IMessagePublisher<in T> where T : new()
    {
        Task PublishAsync(T message);
    }
}
