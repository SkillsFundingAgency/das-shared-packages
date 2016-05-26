using System.Threading.Tasks;

namespace SFA.DAS.Configuration
{
    public interface IConfigurationRepository
    {
        string Get(string serviceName, string environmentName, string version);
        Task<string> GetAsync(string serviceName, string environmentName, string version);
    }
}
