namespace SFA.DAS.VacancyServices.Search
{
    using Entities;
    using Nest;
    using Responses;
    using System.Collections.Generic;
    using System.Linq;

    public class LocationSearchClient : ILocationSearchClient
    {
        public const int MaxResults = 50;
        private readonly IElasticClient _elasticClient;
        private readonly string _indexName;

        internal LocationSearchClient(IElasticClient elasticClient, string indexName)
        {
            _elasticClient = elasticClient;
            _indexName = indexName;
        }

        public LocationSearchResponse Search(string placeName, int maxResults = MaxResults)
        {
            var exactMatchResults = SearchExact(placeName, maxResults);
            var prefixMatchResults = SearchPrefixed(placeName, maxResults);
            var fuzzyMatchResults = SearchFuzzy(placeName, maxResults);

            // Prefer exact matches over prefix matches; prefer prefix matches over fuzzy matches.
            var results =
                exactMatchResults
                    .Concat(prefixMatchResults)
                    .Concat(fuzzyMatchResults)
                    .Distinct((new LocationComparer()))
                    .Take(maxResults)
                    .ToList();

            return new LocationSearchResponse {Locations = results};
        }

        public virtual IEnumerable<LocationSearchResult> SearchExact(string placeName, int maxResults = MaxResults)
        {
            var term = placeName.ToLowerInvariant();

            var exactMatchResults = _elasticClient.Search<LocationSearchResult>(s => s
                .Index(_indexName)
                .Query(q1 => q1
                    .FunctionScore(fs => fs.Query(q2 => q2
                            .Match(m => m.Field(f => f.Name).Query(term)))
                        .Functions(f => f.FieldValueFactor(fvf => fvf.Field(ll => ll.Size))).ScoreMode(FunctionScoreMode.Sum))
                )
                .From(0)
                .Size(maxResults));


            return exactMatchResults.Documents;
        }

        public virtual IEnumerable<LocationSearchResult> SearchPrefixed(string placeName, int maxResults = MaxResults)
        {
            var term = placeName.ToLowerInvariant();

            var prefixMatchResults = _elasticClient.Search<LocationSearchResult>(s => s
                .Index(_indexName)
                .Query(q1 => q1
                    .FunctionScore(fs => fs.Query(q2 => q2
                            .Prefix(p => p.Field(n => n.Name).Value(term)))
                        .Functions(f => f.FieldValueFactor(fvf => fvf.Field(ll => ll.Size))).ScoreMode(FunctionScoreMode.Sum))
                )
                .From(0)
                .Size(maxResults));

            return prefixMatchResults.Documents;
        }

        public virtual IEnumerable<LocationSearchResult> SearchFuzzy(string placeName, int maxResults = MaxResults)
        {
            var term = placeName.ToLowerInvariant();

            var fuzzyMatchResults = _elasticClient.Search<LocationSearchResult>(s => s
                .Index(_indexName)
                .Query(q1 => q1
                    .FunctionScore(fs => fs.Query(q2 =>
                            q2.Fuzzy(f => f.PrefixLength(1).Field(n => n.Name).Value(term).Boost(2.0)) ||
                            q2.Fuzzy(f => f.PrefixLength(1).Field(n => n.County).Value(term).Boost(1.0)))
                        .Functions(f => f.FieldValueFactor(fvf => fvf.Field(ll => ll.Size))).ScoreMode(FunctionScoreMode.Sum))
                )
                .From(0)
                .Size(maxResults));

            return fuzzyMatchResults.Documents;
        }

        private class LocationComparer : IEqualityComparer<LocationSearchResult>
        {
            public bool Equals(LocationSearchResult g1, LocationSearchResult g2)
            {
                return g1.Latitude.Equals(g2.Latitude) &&
                       g1.Longitude.Equals(g2.Longitude);
            }

            public int GetHashCode(LocationSearchResult obj)
            {
                return string.Format("{0},{1}", obj.Longitude, obj.Latitude).ToLower().GetHashCode();
            }
        }
    }
}
