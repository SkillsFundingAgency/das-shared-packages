using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Elastic
{
    public class ElasticClientFactory : IElasticClientFactory
    {
        private readonly ElasticClient _client;
        private readonly ConnectionSettings _settings;

        public ElasticClientFactory(IElasticConfiguration elasticConfig, IEnvironmentConfiguration environmentConfig, IEnumerable<IIndexMapper> indexMappers, ILog log)
        {
            _settings = new ConnectionSettings(new SingleNodeConnectionPool(new Uri(elasticConfig.ElasticUrl)))
                .ThrowExceptions()
                .OnRequestCompleted(r =>
                {
                    log.Debug(r.DebugInformation);
                });

            if (!string.IsNullOrEmpty(elasticConfig.ElasticUsername) && !string.IsNullOrEmpty(elasticConfig.ElasticPassword))
            {
                _settings.BasicAuthentication(elasticConfig.ElasticUsername, elasticConfig.ElasticPassword);
            }

            _client = new ElasticClient(_settings);

            Task.WaitAll(indexMappers.Select(m => m.EnureIndexExists(environmentConfig, _client)).ToArray());
        }

        public IElasticClient GetClient()
        {
            return _client;
        }

        public void Dispose()
        {
            ((IDisposable)_settings).Dispose();
        }
    }
}