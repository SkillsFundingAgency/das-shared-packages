using System.Threading.Tasks;

namespace SFA.DAS.Bus.Client
{
    public interface IBusClient
    {
        Task PublishAsync(object message);
    }
}
