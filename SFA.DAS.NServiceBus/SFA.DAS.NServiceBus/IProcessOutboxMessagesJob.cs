using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus
{
    public interface IProcessOutboxMessagesJob
    {
        Task RunAsync();
    }
}