using System.Threading.Tasks;

namespace SFA.DAS.Configuration
{
    public interface IConfigurationService
    {
        T Get<T>();
        Task<T> GetAsync<T>();
    }
}