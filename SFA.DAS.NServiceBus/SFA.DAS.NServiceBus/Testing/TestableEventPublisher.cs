using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.Testing
{
    public class TestableEventPublisher : IEventPublisher
    {
        public IEnumerable<Event> Events => _events;

        private readonly ConcurrentQueue<Event> _events = new ConcurrentQueue<Event>();

        public Task Publish<T>(T message) where T : Event
        {
            _events.Enqueue(message);

            return Task.CompletedTask;
        }

        public Task Publish<T>(Action<T> action) where T : Event, new()
        {
            var message = new T();

            action(message);
            _events.Enqueue(message);

            return Task.CompletedTask;
        }
    }
}