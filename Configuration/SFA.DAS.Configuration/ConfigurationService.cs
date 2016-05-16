using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ConfigurationOptions _options;

        public ConfigurationService(IConfigurationRepository configurationRepository)
            : this(configurationRepository, new ConfigurationOptions())
        {
        }
        public ConfigurationService(IConfigurationRepository configurationRepository, ConfigurationOptions options)
        {
            _configurationRepository = configurationRepository;
            _options = options;
        }

        public async Task<T> Get<T>()
        {
            var details = await _configurationRepository.Get(_options.ServiceName, _options.EnvironmentName, _options.VersionNumber);
            if (string.IsNullOrEmpty(details))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(details);
        }
    }
}
