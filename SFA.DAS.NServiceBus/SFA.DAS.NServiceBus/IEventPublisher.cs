using System;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus
{
    public interface IEventPublisher
    {
        Task Publish<T>(T message) where T : Event;
        Task Publish<T>(Func<T> messageFactory) where T : Event;
    }
}