using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.UI.Activities.Domain;
using NuGet;
using SFA.DAS.UI.Activities.Domain.Configurations;

namespace SFA.DAS.UI.Activities.DataAccess.Repositories
{
    public class ActivitiesRepository : IActivitiesUIRepository
    {
        private readonly ElasticClient _elasticClient;

        public ActivitiesRepository(ActivitiesConfiguration configuration)
        {
            //var elasticSettings = new ConnectionSettings(new Uri("http://localhost:9200"));
            var elasticSettings = new ConnectionSettings(new Uri(configuration.ElasticServerBaseUrl));
            _elasticClient = new ElasticClient(elasticSettings);

        }

        public async Task<IEnumerable<Activity>> GetActivities(string accountId, string type)
        {
            var searchResponse = await _elasticClient.SearchAsync<Activity>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.AccountId)
                        .Query(accountId)
                    )
                )
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.ActivityType)
                        .Query(type)
                    )
                )
            );

            return searchResponse.Documents;
        }

        public async Task<IEnumerable<Activity>> GetActivities(string ownerId)
        {
            var searchResponse = await _elasticClient.SearchAsync<Activity>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.AccountId)
                        .Query(ownerId)
                    )
                )
            );

            return searchResponse.Documents;
        }

        public async Task<Activity> GetActivity(Activity activity)
        {
            var searchResponse = await _elasticClient.SearchAsync<Activity>(s => s
                .From(0)
                .Size(1)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.AccountId)
                        .Query(activity.AccountId)
                    )
                )
                .Query(q => q
                    .Match(m => m
                    .Field(f => f.ActivityType)
                    .Query(activity.ActivityType)
                    )
                )
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Description)
                        .Query(activity.Description)
                    )
                )
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Url)
                        .Query(activity.Url)
                    )
                )
            );

            return searchResponse.Documents.FirstOrDefault();
        }

        public async Task SaveActivity(Activity activity)
        {
            var activityAlreadyExists = await GetActivity(activity);

            if (activityAlreadyExists == null)
                await (_elasticClient.IndexAsync(activity));
        }
    }
}
