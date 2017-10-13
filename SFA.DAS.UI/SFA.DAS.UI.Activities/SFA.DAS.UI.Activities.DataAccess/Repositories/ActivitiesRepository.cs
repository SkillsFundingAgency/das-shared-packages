﻿using System;
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
            //var elasticSettings = new ConnectionSettings(new Uri("http://localhost:9200"));
            var elasticSettings = new ConnectionSettings(new Uri(configuration.ElasticServerBaseUrl));
            _elasticClient = new ElasticClient(elasticSettings);

        }

        public async Task<IEnumerable<Activity>> GetActivities(string ownerId)
        {
            var searchResponse = await _elasticClient.SearchAsync<Activity>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.OwnerId)
                        .Query(ownerId)
                    )
                )
            );

            return searchResponse.Documents;
        }
    }
}
