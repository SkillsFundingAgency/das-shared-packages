using Elasticsearch.Net;
using Nest;
using SFA.DAS.VacancyServices.Search.Configuration;
using System;

namespace SFA.DAS.VacancyServices.Search.Clients
{
    public class ElasticsearchClientFactory : IElasticSearchClientFactory
    {
        private readonly ConnectionSettings _connectionSettings;

        public ElasticsearchClientFactory(SearchConfiguration searchConfiguration)
        {
            var elasticsearchConfiguration = searchConfiguration;

            var connectionPool = new SingleNodeConnectionPool(new Uri(elasticsearchConfiguration.HostName));
            var connection = new HttpConnection();
            _connectionSettings =
                new ConnectionSettings(connectionPool, connection)
                .BasicAuthentication(elasticsearchConfiguration.UserName, elasticsearchConfiguration.Password);

#if DEBUG            
            //formats the json in the DebugInformation to make it readable
            _connectionSettings.EnableDebugMode();
#endif
        }

        public IElasticClient GetElasticClient()
        {
            var client = new ElasticClient(_connectionSettings);
            return client;
        }
    }
}
