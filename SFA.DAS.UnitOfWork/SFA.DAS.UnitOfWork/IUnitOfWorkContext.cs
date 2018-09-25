using System;
using System.Collections.Generic;

namespace SFA.DAS.UnitOfWork
{
    public interface IUnitOfWorkContext
    {
        void AddEvent<T>(T message) where T : class;
        void AddEvent<T>(Func<T> messageFactory) where T : class;
        T Get<T>();
        IEnumerable<object> GetEvents();
        void Set<T>(T value);
        T TryGet<T>() where T : class;
    }
}