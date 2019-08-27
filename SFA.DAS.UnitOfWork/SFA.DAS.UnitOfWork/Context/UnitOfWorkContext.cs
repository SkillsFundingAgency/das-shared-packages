﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SFA.DAS.UnitOfWork.Context
{
    public class UnitOfWorkContext : IUnitOfWorkContext
    {
        private static readonly AsyncLocal<ConcurrentQueue<Func<object>>> Events = new AsyncLocal<ConcurrentQueue<Func<object>>>();

        private readonly ConcurrentDictionary<string, object> _data = new ConcurrentDictionary<string, object>();

        public UnitOfWorkContext()
        {
            Events.Value = new ConcurrentQueue<Func<object>>();
        }

        public static void AddEvent<T>(T message) where T : class
        {
            Events.Value.Enqueue(() => message);
        }

        public static void AddEvent<T>(Func<T> messageFactory) where T : class
        {
            Events.Value.Enqueue(messageFactory);
        }

        void IUnitOfWorkContext.AddEvent<T>(T message)
        {
            AddEvent(message);
        }

        void IUnitOfWorkContext.AddEvent<T>(Func<T> action)
        {
            AddEvent(action);
        }

        public T Find<T>() where T : class
        {
            return _data.TryGetValue(typeof(T).FullName, out var value) ? (T)value : null;
        }

        public T Get<T>() where T : class
        {
            var key = typeof(T).FullName;

            if (_data.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            throw new KeyNotFoundException($"The key '{key}' was not present in the unit of work context");
        }

        public IEnumerable<object> GetEvents()
        {
            return Events.Value.Select(e => e());
        }

        public void Set<T>(T value) where T : class
        {
            _data[typeof(T).FullName] = value;
        }
    }
}