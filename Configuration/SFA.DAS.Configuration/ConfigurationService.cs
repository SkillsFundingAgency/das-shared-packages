using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ConfigurationOptions _options;
        
        public ConfigurationService(IConfigurationRepository configurationRepository, ConfigurationOptions options)
        {
            _configurationRepository = configurationRepository;
            _options = options;
        }

        public T Get<T>()
        {
            var details = _configurationRepository.Get(_options.ServiceName, _options.EnvironmentName, _options.VersionNumber);
            return ParseConfig<T>(details);
        }
        public async Task<T> GetAsync<T>()
        {
            var details = await _configurationRepository.GetAsync(_options.ServiceName, _options.EnvironmentName, _options.VersionNumber);
            return ParseConfig<T>(details);
        }

        private T ParseConfig<T>(string details)
        {
            if (string.IsNullOrEmpty(details))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(details);
        }
    }
}
