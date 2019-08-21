using System;
using System.Threading.Tasks;

namespace SFA.DAS.Caches
{
    public interface IInProcessCache
    {
        bool Exists(string key);
        T Get<T>(string key);
        void Set(string key, object value);
        void Set(string key, object value, TimeSpan slidingExpiration);
        void Set(string key, object value, DateTimeOffset absoluteExpiration);
        Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter, DateTimeOffset absoluteExpiration);
    }
}
