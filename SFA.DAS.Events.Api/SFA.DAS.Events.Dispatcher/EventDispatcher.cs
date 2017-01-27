using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Dispatcher
{
    public class EventDispatcher : IEventDispatcher
    {
        private List<EventRegistration> _registrations = new List<EventRegistration>();

        public async Task Dispatch<T>(T @event) where T : IEventView
        {
            var matchedRegistrations = FindMatchingRegistrations(@event);

            await ExecuteHandlers(@event, matchedRegistrations);
        }

        private static async Task ExecuteHandlers<T>(T @event, IEnumerable<EventRegistration> matchedRegistrations) where T : IEventView
        {
            foreach (var registration in matchedRegistrations)
            {
                var handler = (IEventHandler<T>)registration.Handler;
                await handler.Handle(@event);
            }
        }

        private IEnumerable<EventRegistration> FindMatchingRegistrations<T>(T @event) where T : IEventView
        {
            var matchedRegistrations = _registrations.Where(h => h.EventType == typeof(T) && h.EventName == @event.Event).ToList();

            if (!matchedRegistrations.Any())
            {
                throw new HandlerNotRegisteredException(typeof(T), @event.Event);
            }
            return matchedRegistrations;
        }

        public async Task RegisterHandler<T>(IEventHandler<T> handler, string @event) where T : IEventView
        {
            _registrations.Add(new EventRegistration(typeof(T), @event, handler));
        }
    }
}
