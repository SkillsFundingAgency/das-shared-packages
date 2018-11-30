namespace SFA.DAS.VacancyServices.Search
{
    using System;
    using Nest;
    using Newtonsoft.Json.Converters;

    internal class ElasticSearchFactory : IElasticSearchFactory
    {
        public IElasticClient GetElasticClient(string hostName)
        {
            var elasticConnectionSettings = new ConnectionSettings(new Uri(hostName));
            elasticConnectionSettings.AddContractJsonConverters(t => typeof(Enum).IsAssignableFrom(t) ? new StringEnumConverter() : null);

            return new ElasticClient(elasticConnectionSettings);
        }
    }
}
