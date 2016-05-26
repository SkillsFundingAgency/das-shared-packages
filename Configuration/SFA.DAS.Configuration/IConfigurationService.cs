using System.Threading.Tasks;

namespace SFA.DAS.Configuration
{
    public interface IConfigurationService
    {
        Task<T> GetAsync<T>();
    }
}