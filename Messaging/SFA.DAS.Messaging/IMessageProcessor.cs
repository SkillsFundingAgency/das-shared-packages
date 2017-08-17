using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public interface IMessageProcessor
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
