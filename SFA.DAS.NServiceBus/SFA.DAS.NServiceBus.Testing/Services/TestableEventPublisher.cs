using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.NServiceBus.Testing.Services
{
    public class TestableEventPublisher : IEventPublisher
    {
        public IEnumerable<object> Events => _events;

        private readonly ConcurrentQueue<object> _events = new ConcurrentQueue<object>();

        public Task Publish<T>(T message) where T : class
        {
            _events.Enqueue(message);

            return Task.CompletedTask;
        }

        public Task Publish<T>(Func<T> messageFactory) where T : class
        {
            var message = messageFactory();
            
            _events.Enqueue(message);

            return Task.CompletedTask;
        }
    }
}