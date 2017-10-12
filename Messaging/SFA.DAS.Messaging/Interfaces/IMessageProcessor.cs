using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Interfaces
{
    public interface IMessageProcessor
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
