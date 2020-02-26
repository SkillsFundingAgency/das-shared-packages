using System;
using Elasticsearch.Net;
using Nest;

namespace SFA.DAS.Elastic
{
    public sealed class ElasticClientFactory : IElasticClientFactory
    {
        internal readonly ElasticClientConfiguration _configuration;

        internal ElasticClientFactory(ElasticClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IElasticClient CreateClient()
        {
            var connectionSettings = _configuration.IsCloudConnectionConfigured ? GetCloudBasedConnectionSettings(_configuration) : GetSingleNodeBasedConnectionSettings(_configuration);

            if (_configuration.OnRequestCompleted != null) connectionSettings.OnRequestCompleted(_configuration.OnRequestCompleted);

            if (_configuration.EnableDebugMode) connectionSettings.EnableDebugMode();

            connectionSettings.ThrowExceptions();

            var client = new ElasticClient(connectionSettings);

            return client;
        }

        private static ConnectionSettings GetCloudBasedConnectionSettings(ElasticClientConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.CloudId)
                || string.IsNullOrWhiteSpace(configuration.Username)
                || string.IsNullOrWhiteSpace(configuration.Password))
            {
                throw new Exception("The cloudid, username and password is required in the search configuration.");
            }

            var credentials = new BasicAuthenticationCredentials(configuration.Username, configuration.Password);
            var connectionPool = new CloudConnectionPool(configuration.CloudId, credentials);

            return new ConnectionSettings(connectionPool);
        }

        private static ConnectionSettings GetSingleNodeBasedConnectionSettings(ElasticClientConfiguration configuration)
        {
            var connectionPool = new SingleNodeConnectionPool(configuration.HostUri);
            var connectionSettings = new ConnectionSettings(connectionPool);
            connectionSettings.BasicAuthentication(configuration.Username, configuration.Password);
            return connectionSettings;
        }
    }
}