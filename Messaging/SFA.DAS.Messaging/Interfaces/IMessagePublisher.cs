using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync(object message);
    }
}
