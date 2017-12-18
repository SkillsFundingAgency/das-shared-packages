using System;

namespace SFA.DAS.Storage
{
    public interface IConfigurationInfo<T>
    {
        T GetConfiguration(string serviceName);
        T GetConfiguration(string serviceName, Action<string> action);
    }
}