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
        public string EnvironmentName { get; }
        public Func<IEnumerable<IIndexMapper>> IndexMappersFactory { get; }
        public IConnectionSettingsValues ConnectionSettings { get; }

        public ElasticClientFactory(string environmentName, string url, string username, string password, Action<IApiCallDetails> onRequestCompleted, Func<IEnumerable<IIndexMapper>> indexMappersFactory)
        {
            var connectionSettings = new ConnectionSettings(new SingleNodeConnectionPool(new Uri(url))).ThrowExceptions();

            if (onRequestCompleted != null)
            {
                connectionSettings.OnRequestCompleted(onRequestCompleted);
            }

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                connectionSettings.BasicAuthentication(username, password);
            }

            EnvironmentName = environmentName;
            IndexMappersFactory = indexMappersFactory;
            ConnectionSettings = connectionSettings;
        }

        public IElasticClient CreateClient()
        {
            var client = new ElasticClient(ConnectionSettings);

            if (IndexMappersFactory != null)
            {
                var indexMappers = IndexMappersFactory();
                var tasks = indexMappers.Select(m => m.EnureIndexExistsAsync(EnvironmentName, client)).ToArray();

                Task.WaitAll(tasks);
            }

            return client;
        }

        public void Dispose()
        {
            ConnectionSettings.Dispose();
        }
    }
}