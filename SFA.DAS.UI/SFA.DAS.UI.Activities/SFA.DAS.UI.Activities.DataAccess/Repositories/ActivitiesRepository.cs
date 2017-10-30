using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.UI.Activities.Domain;
using NuGet;
using SFA.DAS.UI.Activities.Domain.Configurations;

namespace SFA.DAS.UI.Activities.DataAccess.Repositories
{
    public class ActivitiesRepository : IActivitiesUiRepository
    {
        private readonly ElasticClient _elasticClient;

        public ActivitiesRepository(ActivitiesConfiguration configuration)
        {
            var elasticSettings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("activities");
            //var elasticSettings = new ConnectionSettings(new Uri(configuration.ElasticServerBaseUrl));

            //var descriptor = new CreateIndexDescriptor("activity")
            //    .Mappings(ms => ms
            //        .Map<Activity>(m => m
            //        .Properties(ps=>ps
            //        .Text(s=>s.Name(a=>a.OwnerId)
            //        ()))
            //    );
            _elasticClient = new ElasticClient(elasticSettings);


        }

        public IEnumerable<Activity> GetActivities(string ownerId)
        {
            var searchResponse = _elasticClient.Search<Activity>(s => s
            .Index("activities")
            .Type(typeof(Activity))
            .MatchAll()
                //.Query(q => q
                //    .Match(m => m
                //        .Field(f => f.OwnerId)
                //        .Query(ownerId)
                //    )
                //)
            );

            var a = searchResponse.Documents;

            return searchResponse.Documents;
        }

        public IEnumerable<Activity> GetActivitiesGroupedByDayAndType(string ownerId)
        {
            var searchResponse = _elasticClient.Search<Activity>(s => s
                    .Index("activities")
                    .Type(typeof(Activity))
                    .MatchAll()
                //.Query(q => q
                //    .Match(m => m
                //        .Field(f => f.OwnerId)
                //        .Query(ownerId)
                //    )
                //)
            );

            var a = searchResponse.Documents;

            return searchResponse.Documents;
        }


        //public async Task<IEnumerable<Activity>> GetLatestActivitiesGrouped(string ownerId)
        //{
        //    var searchResponse = await _elasticClient.SearchAsync<Activity>(s => s
        //        .From(0)
        //        .Size(10)
        //        .Aggregations(a=>a.ValueCount("name",c=>c.Field(p=>p.Type)

        //            )
        //        ) )
        //    );

        //    return searchResponse.Documents;
        //}

    }
}
