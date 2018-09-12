using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SFA.DAS.UnitOfWork
{
    public class UnitOfWorkContext : IUnitOfWorkContext
    {
        private static readonly AsyncLocal<ConcurrentQueue<Func<object>>> Events = new AsyncLocal<ConcurrentQueue<Func<object>>>();

        private readonly ConcurrentDictionary<string, object> _data = new ConcurrentDictionary<string, object>();

        public UnitOfWorkContext()
        {
            Events.Value = new ConcurrentQueue<Func<object>>();
        }

        public static void AddEvent(object message)
        {
            Events.Value.Enqueue(() => message);
        }

        public static void AddEvent<T>(Action<T> action) where T : class, new()
        {
            Events.Value.Enqueue(() =>
            {
                var message = new T();

                action(message);

                return message;
            });
        }

        void IUnitOfWorkContext.AddEvent(object message)
        {
            AddEvent(message);
        }

        void IUnitOfWorkContext.AddEvent<T>(Action<T> action)
        {
            AddEvent(action);
        }

        public T Get<T>()
        {
            var key = typeof(T).FullName;

            if (_data.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            throw new KeyNotFoundException($"The key '{key}' was not present in the dictionary");
        }

        public IEnumerable<object> GetEvents()
        {
            return Events.Value.Select(e => e());
        }

        public void Set<T>(T value)
        {
            _data[typeof(T).FullName] = value;
        }

        public T TryGet<T>() where T : class
        {
            var key = typeof(T).FullName;

            if (_data.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return null;
        }
    }
}