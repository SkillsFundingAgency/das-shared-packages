namespace SFA.DAS.VacancyServices.Search
{
    using System;
    using Elasticsearch.Net;
    using Nest;
    using Newtonsoft.Json.Converters;

    internal class ElasticSearchFactory : IElasticSearchFactory
    {
        public IElasticClient GetElasticClient(ApprenticeshipSearchClientConfiguration config)
        {
            return GetElasticClient(config.HostName, config.Username, config.Password);
        }

        public IElasticClient GetElasticClient(TraineeshipSearchClientConfiguration config)
        {
            return GetElasticClient(config.HostName, config.Username, config.Password);
        }

        public IElasticClient GetElasticClient(LocationSearchClientConfiguration config)
        {
            return GetElasticClient(config.HostName, config.Username, config.Password);
        }

        private IElasticClient GetElasticClient(string hostname, string username, string password)
        {
            var nodePool = new SingleNodeConnectionPool(new Uri(hostname));
            var elasticConnectionSettings = new ConnectionSettings(nodePool)
                .BasicAuthentication(username, password);
            return new ElasticClient(elasticConnectionSettings);
        }
    }
}
