using Elasticsearch.Net;
using System;

namespace SFA.DAS.Elastic.Extensions
{
    public static class ElasticClientConfigurationExtensions
    {
        public static ElasticClientConfiguration SetOnRequestCompletedCallbackAction(this ElasticClientConfiguration configuration, Action<IApiCallDetails> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            configuration.OnRequestCompleted = handler;

            return configuration;
        }

        public static ElasticClientConfiguration SetDebugMode(this ElasticClientConfiguration configuration)
        {
            configuration.EnableDebugMode = true;

            return configuration;
        }

        public static IElasticClientFactory CreateClientFactory(this ElasticClientConfiguration configuration)
        {
            return new ElasticClientFactory(configuration);
        }
    }
}
