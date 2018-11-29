using System;

namespace SFA.DAS.Testing.Builders
{
    public static class ObjectActivator
    {
        internal static T CreateInstance<T>() where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), true);
        }
    }
}