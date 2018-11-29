using System;

namespace SFA.DAS.Testing.Builders
{
    public static class ObjectActivator
    {
        public static T CreateInstance<T>() where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), true);
        }
    }
}