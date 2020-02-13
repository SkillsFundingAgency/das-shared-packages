namespace SFA.DAS.VacancyServices.Search
{
    using Requests;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Elasticsearch.Net;
    using Entities;
    using Nest;
    using Responses;

    public class TraineeshipSearchClient : ITraineeshipSearchClient
    {
        private const string ScrollIndexConsistencyTime = "5s";
        private const int ScrollSize = 100;
        private const string ScrollTimeout = "5s";

        private readonly IElasticSearchFactory _elasticSearchFactory;
        private readonly TraineeshipSearchClientConfiguration _config;

        public TraineeshipSearchClient(TraineeshipSearchClientConfiguration config)
            : this(new ElasticSearchFactory(), config)
        {
        }

        internal TraineeshipSearchClient(IElasticSearchFactory elasticSearchFactory, TraineeshipSearchClientConfiguration config)
        {
            _elasticSearchFactory = elasticSearchFactory;
            _config = config;
        }

        public IEnumerable<int> GetAllVacancyIds()
        {
            var client = _elasticSearchFactory.GetElasticClient(_config);

            var scanResults = client.Search<TraineeshipSearchResult>(search => search
                .Index(_config.Index)
                .From(0)
                .Size(ScrollSize)
                .MatchAll()
                .Scroll(ScrollIndexConsistencyTime));

            var vacancyIds = new List<int>();
            var scrollRequest = new ScrollRequest(scanResults.ScrollId, ScrollTimeout);

            while (true)
            {
                var results = client.Scroll<TraineeshipSearchResult>(scrollRequest);

                if (!results.Documents.Any())
                {
                    break;
                }

                vacancyIds.AddRange(results.Documents.Select(each => each.Id));
            }

            return vacancyIds;
        }

        public TraineeshipSearchResponse Search(TraineeshipSearchRequestParameters searchParameters)
        {
            var results = PerformSearch(searchParameters);

            var response = new TraineeshipSearchResponse(results.Total, results.Documents, searchParameters);

            return response;
        }
        
        private ISearchResponse<TraineeshipSearchResult> PerformSearch(TraineeshipSearchRequestParameters parameters)
        {
            var client = _elasticSearchFactory.GetElasticClient(_config);

            var results = client.Search<TraineeshipSearchResult>(s =>
            {
                s.Index(_config.Index);
                s.Skip((parameters.PageNumber - 1) * parameters.PageSize);
                s.Take(parameters.PageSize);

                s.TrackScores();

                s.Query(q => GetQuery(parameters, q));

                SetSort(s, parameters);

                return s;
            });

            SetHitValuesOnSearchResults(parameters, results);

            return results;
        }

        private QueryContainer GetQuery(TraineeshipSearchRequestParameters parameters, QueryContainerDescriptor<TraineeshipSearchResult> q)
        {
            if (!string.IsNullOrEmpty(parameters.VacancyReference))
            {
                return q.Bool(fq =>
                    fq.Filter(f =>
                        f.Term(t =>
                            t.VacancyReference, parameters.VacancyReference)));
            }

            QueryContainer query = null;

            if (parameters.DisabilityConfidentOnly)
            {
                var queryDisabilityConfidentOnly = q
                    .Match(m => m.Field(f => f.IsDisabilityConfident)
                        .Query(parameters.DisabilityConfidentOnly.ToString()));
                query &= queryDisabilityConfidentOnly;
            }

            if (parameters.Ukprn.HasValue)
            {
                var queryClause = q
                    .Match(m => m.Field(f => f.Ukprn)
                        .Query(parameters.Ukprn.ToString()));
                query &= queryClause;
            }

            if (parameters.CanFilterByGeoDistance)
            {
                var queryClause = q.Bool(qf => qf.Filter(f => f
                    .GeoDistance(vs => vs
                        .Location(parameters.Latitude.Value, parameters.Longitude.Value)
                        .Distance(parameters.SearchRadius.Value, DistanceUnit.Miles))));
                query &= queryClause;
            }

            return query;
        }

        private static void SetSort(SearchDescriptor<TraineeshipSearchResult> search, TraineeshipSearchRequestParameters parameters)
        {
            switch (parameters.SortType)
            {
                case VacancySearchSortType.RecentlyAdded:
                    search.Sort(s => s.Descending(r => r.PostedDate));
                    search.TrySortByGeoDistance(parameters);
                    break;
                case VacancySearchSortType.Distance:
                    search.TrySortByGeoDistance(parameters);
                    break;
                case VacancySearchSortType.ClosingDate:
                    search.Sort(s => s.Ascending(r => r.ClosingDate));
                    search.TrySortByGeoDistance(parameters);
                    break;
                default:
                    search.Sort(s => s.Descending(r => r.Score));
                    search.TrySortByGeoDistance(parameters);
                    break;
            }
        }

        private static void SetHitValuesOnSearchResults(TraineeshipSearchRequestParameters searchParameters, ISearchResponse<TraineeshipSearchResult> results)
        {
            foreach (var result in results.Documents)
            {
                var hitMd = results.Hits.First(h => h.Id == result.Id.ToString(CultureInfo.InvariantCulture));

                if (searchParameters.CanSortByGeoDistance)
                    result.Distance = (double)hitMd.Sorts.ElementAt(GetGeoDistanceSortHitPosition(searchParameters));

                result.Score = hitMd.Score.GetValueOrDefault(0);
            }
        }

        private static int GetGeoDistanceSortHitPosition(TraineeshipSearchRequestParameters parameters)
        {
            switch (parameters.SortType)
            {
                case VacancySearchSortType.Distance:
                    return 0;
                default:
                    return 1;
            }
        }
    }
}
