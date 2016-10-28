using System;

namespace SFA.DAS.Configuration
{
    public interface IConfigurationCache
    {
            T Get<T>(string key);
            void Set(string key, object value);
    }
}
