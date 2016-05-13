using System.Threading.Tasks;

namespace SFA.DAS.Configuration
{
    public interface IConfigurationRepository
    {
        Task<string> Get(string serviceName, string environmentName, string version);
    }
}
