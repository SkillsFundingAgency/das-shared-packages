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

        public IReadOnlyCollection<Hit<Activity>> GetAggregations(long accountId)
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
                .Aggregations(agg => 
                    agg.TopHits("top_tags", b => b.Field(f => "typeOfActivityKeyword")
                .Size(3)
                ))
            );

            if (searchResponse.DebugInformation.Contains("Invalid"))
                throw new InvalidOperationException();

            var things = searchResponse.Aggs.TopHits("top_tags");

            return things.Hits<Activity>();
        }

        public ISearchResponse<Activity> GetAggregations2(long accountId)
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
                .Aggregations(agg => agg
                    .Terms("groupId", b => b.
                        Field(f => "typeOfActivityKeyword")
                        //.MinimumDocumentCount(0)))
                        .Size(0)))
                        .Aggregations(tagdaggs=> tagdaggs
                            .TopHits("top_tag_hits", thagd => thagd
                            .Sort(a=>a
                                .Field(p=>p.PostedDateTimeKeyword)
                                .Order(SortOrder.Descending)
                                )
                  ))
            );

            if (searchResponse.DebugInformation.Contains("Invalid"))
                throw new InvalidOperationException();


            return searchResponse;
        }

        public ISearchResponse<Activity> GetAggregations3(long accountId)
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

        //Equivalent for postman
        //{
        //    "size" : 0,
        //    "aggs": {
        //        "activity": {
        //            "terms": {
        //                "field": "typeOfActivityKeyword"
        //            },
        //            "aggs": {
        //                "date": {
        //                    "terms": {
        //                        "field": "postedDateKeyword"
        //                    }
        //                }
        //            }
    
        //        }
        //    }
        //}

    //public IReadOnlyCollection<Nest.KeyedBucket<string>> GetAggregations2(string accountId)
    //{
    //    var searchResponse = _elasticClient.Search<Activity>(s => s
    //        .Index("activities")
    //        .Type(typeof(Activity))
    //        .MatchAll()
    //        .Aggregations(agg => agg.Terms("whatIcallType", b => b.Field(f => "type")
    //            .Size(10)))
    //    );

    //    if (searchResponse.DebugInformation.Contains("Invalid"))
    //        throw new InvalidOperationException();

    //    var things = searchResponse.Aggs.Terms("whatIcallType");

    //    return searchResponse.Aggregations.
    //}

    public IEnumerable<Activity> GetActivitiesGroupedByDayAndType(long accountId)
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
                    .Aggregations(agg => agg.Terms("Type", b => b.Field(f => "typeOfActivityKeyword")))
            );

            if (searchResponse.DebugInformation.Contains("Invalid"))
                throw new InvalidOperationException();

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
