using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    public interface IProcessClientOutboxMessagesJob
    {
        Task RunAsync();
    }
}