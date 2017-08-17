using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    interface IMessageProcessor
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
