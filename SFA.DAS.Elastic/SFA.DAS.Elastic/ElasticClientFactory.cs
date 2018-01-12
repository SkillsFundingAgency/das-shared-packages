using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace SFA.DAS.Elastic
{
    public class ElasticClientFactory : IElasticClientFactory
    {
        private readonly string _environmentName;
        private readonly Func<IEnumerable<IIndexMapper>> _indexMappersFactory;
        private readonly ConnectionSettings _settings;

        public ElasticClientFactory(string environmentName, string url, string username, string password, Action<IApiCallDetails> onRequestCompleted, Func<IEnumerable<IIndexMapper>> indexMappersFactory)
        {
            _environmentName = environmentName;
            _indexMappersFactory = indexMappersFactory;

            _settings = new ConnectionSettings(new SingleNodeConnectionPool(new Uri(url))).ThrowExceptions();

            if (onRequestCompleted != null)
            {
                _settings.OnRequestCompleted(onRequestCompleted);
            }

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                _settings.BasicAuthentication(username, password);
            }
        }

        public IElasticClient CreateClient()
        {
            var client = new ElasticClient(_settings);

            if (_indexMappersFactory != null)
            {
                var indexMappers = _indexMappersFactory();
                var tasks = indexMappers.Select(m => m.EnureIndexExistsAsync(_environmentName, client)).ToArray();

                Task.WaitAll(tasks);
            }

            return client;
        }

        public void Dispose()
        {
            ((IDisposable)_settings).Dispose();
        }
    }
}