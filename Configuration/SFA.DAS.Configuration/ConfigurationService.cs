using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ConfigurationOptions _options;
        private readonly IConfigurationCache _configurationCache;

        public ConfigurationService(IConfigurationRepository configurationRepository, ConfigurationOptions options)
            : this(configurationRepository, options, null)
        { }

        public ConfigurationService(IConfigurationRepository configurationRepository, ConfigurationOptions options, 
            IConfigurationCache configurationCache)
        {
            _configurationRepository = configurationRepository;
            _options = options;
            _configurationCache = configurationCache;
        }

        public T Get<T>()
        {
            var details = _configurationCache?.Get<string>(typeof(T).FullName);

            if (!string.IsNullOrEmpty(details))
                return ParseConfig<T>(details);

            details = _configurationRepository.Get(_options.ServiceName, _options.EnvironmentName, _options.VersionNumber);

            if (!string.IsNullOrEmpty(details))
                _configurationCache?.Set(typeof(T).FullName, details);

            return ParseConfig<T>(details);
        }
        public async Task<T> GetAsync<T>()
        {
            var details = _configurationCache?.Get<string>(typeof(T).FullName);

            if (!string.IsNullOrEmpty(details))
                return ParseConfig<T>(details);

            details = await _configurationRepository.GetAsync(_options.ServiceName, _options.EnvironmentName, _options.VersionNumber);

            if (!string.IsNullOrEmpty(details))
                _configurationCache?.Set(typeof(T).FullName, details);

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
