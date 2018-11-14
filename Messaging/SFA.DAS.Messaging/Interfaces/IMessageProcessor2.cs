using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Interfaces
{
    public interface IMessageProcessor2
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}