namespace SFA.DAS.VacancyServices.Search
{
    using System;
    using Elasticsearch.Net;
    using Nest;
    using Newtonsoft.Json.Converters;

    internal class ElasticSearchFactory : IElasticSearchFactory
    {      
        private ConnectionSettings SetupCloudConnection(ElasticClientConfigurationBase configuration)
        {
            if (string.IsNullOrEmpty(configuration.CloudId)
                || string.IsNullOrWhiteSpace(configuration.Username)
                || string.IsNullOrWhiteSpace(configuration.Password))
            {
                throw new Exception("The cloudid, username and password is required in the search configuration.");
            }

            var credentials = new BasicAuthenticationCredentials(configuration.Username,
                   configuration.Password);
            var connectionPool = new CloudConnectionPool(configuration.CloudId, credentials);
            return new ConnectionSettings(connectionPool);
        }

        private ConnectionSettings SetupLocalConnection(ElasticClientConfigurationBase configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration.HostName))
            {
                throw new Exception("HostName in Search Configuration is required.");
            }
            var connectionPool = new SingleNodeConnectionPool(new Uri(configuration.HostName));
            var connectionSettings = new ConnectionSettings(connectionPool);
            connectionSettings.EnableDebugMode();
            return connectionSettings;
        }

        public IElasticClient GetElasticClient(ElasticClientConfigurationBase configuration)
        {
            ConnectionSettings connectionSettings;
#if DEBUG
            connectionSettings = SetupLocalConnection(configuration);
#else
            connectionSettings = SetupCloudConnection(configuration);   
#endif
            var client = new ElasticClient(connectionSettings);
            return client;
        }
    }
}
