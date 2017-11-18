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

        public ISearchResponse<Activity> GetAggregationsByDay(long accountId)
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
                .Aggregations(a => a
                    .Terms("keywordsBuckets", t => t
                        .Field(p => p.PostedDateKeyword)
                        .Order(TermsOrder.TermDescending)
                        )
                    )
                );

            if (searchResponse.DebugInformation.Contains("Invalid"))
                throw new InvalidOperationException();


            return searchResponse;
        }

        public ISearchResponse<Activity> GetAggregationsByType(long accountId)
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
                .Aggregations(a => a
                    .Terms("keywordsBuckets", t => t
                        .Field(p => p.TypeOfActivityKeyword)
                        .Aggregations(aa => aa
                            .Terms("dateBuckets", th => th
                                    .Field(p => p.PostedDateTimeKeyword)
                                )
                            //.Source(src => src
                            //    .Includes(fs => fs
                            //        .Field(p => p.OwnerId)
                            //        //.Field(p => p.)
                            //    )
                            //)
                            //.Size(1)
                            //.Version()
                            //.Explain()
                            //.FielddataFields(fd => fd
                            //    .Field(p => p.State)
                            //    .Field(p => p.NumberOfCommits)
                            //)
                            //.Highlight(h => h
                            //    .Fields(
                            //        hf => hf.Field(p => p.Tags),
                            //        hf => hf.Field(p => p.Description)
                            //    )
                            //)
                            //.ScriptFields(sfs => sfs
                            //    .ScriptField("commit_factor", sf => sf
                            //        .Inline("doc['numberOfCommits'].value * 2")
                            //        .Lang("groovy")
                            //    )
                            //)
                            )
                        )
                    )
                );

            if (searchResponse.DebugInformation.Contains("Invalid"))
                throw new InvalidOperationException();


            return searchResponse;
        }

    }
}
