using System;
using Elasticsearch.Net;
using Nest;

namespace SFA.DAS.Elastic
{
    public interface IElasticClientFactory
    {
        IElasticClient CreateClient();
        IElasticClient CreateClient(Action<IApiCallDetails> callbackAction);
    }
}