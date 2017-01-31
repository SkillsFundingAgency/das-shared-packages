using System.Threading.Tasks;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Dispatcher
{
    public interface IEventDispatcher
    {
        Task Dispatch<T>(T @event) where T : IEventView;

        Task RegisterHandler<T>(IEventHandler<T> handler, string @event) where T : IEventView;
    }
}
