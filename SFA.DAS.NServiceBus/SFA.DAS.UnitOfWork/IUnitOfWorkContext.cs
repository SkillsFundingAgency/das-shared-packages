using System;
using System.Collections.Generic;

namespace SFA.DAS.UnitOfWork
{
    public interface IUnitOfWorkContext
    {
        void AddEvent(object message);
        void AddEvent<T>(Action<T> action) where T : class, new();
        T Get<T>();
        IEnumerable<object> GetEvents();
        void Set<T>(T value);
    }
}