using System;
using System.Collections.Generic;
using Nest;
using NuGet;
using SFA.DAS.UI.Activities.Domain.Configurations;

namespace SFA.DAS.UI.Activities.DataAccess.Repositories
{
    public class ActivitiesRepository : IActivitiesUiRepository
    {
        private readonly ElasticClient _elasticClient;

        public ActivitiesRepository(ActivitiesConfiguration configuration)
        {
            var elasticSettings = new ConnectionSettings(new Uri("http://activities_app:3t&98Fo5Z00r@localhost:9200")).DefaultIndex("activities");
            //var elasticSettings = new ConnectionSettings(new Uri(configuration.ElasticServerBaseUrl)).DefaultIndex("activities");

            _elasticClient = new ElasticClient(elasticSettings);


        }

        public IEnumerable<Activity> GetActivities(long accountId)
        {
            var searchResponse = _elasticClient.Search<Activity>(s => s
                .Index("activities")
                .Type(typeof(Activity))
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.AccountId)
                        .Query(accountId.ToString())
                    )
                )
            );

            return searchResponse.Documents;
        }

       

    }
}
