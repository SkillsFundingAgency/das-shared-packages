using System;

namespace SFA.DAS.Configuration.AzureTableStorage.Environment
{
    public interface IConfigurationInfo<T>
    {
        T GetConfiguration(string serviceName);
        T GetConfiguration(string serviceName, Action<string> action);
    }
}