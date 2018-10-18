using System;
using System.Collections.Generic;

namespace SFA.DAS.UnitOfWork
{
    public interface IUnitOfWorkContext
    {
        void AddEvent<T>(T message) where T : class;
        void AddEvent<T>(Func<T> messageFactory) where T : class;
        T Find<T>() where T : class;
        T Get<T>() where T : class;
        IEnumerable<object> GetEvents();
        void Set<T>(T value) where T : class;
    }
}