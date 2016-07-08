using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync(object message);
    }
}
