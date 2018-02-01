using System;

namespace SFA.DAS.ApiSubstitute.Utilities
{
    public interface IObjectCreator
    {
        object Create(Type type, object properties = null);

        T Create<T>(Action<T> properties = null) where T : class, new();
    }
}
