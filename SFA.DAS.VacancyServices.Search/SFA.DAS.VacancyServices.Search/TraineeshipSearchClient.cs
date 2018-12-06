using SFA.DAS.VacancyServices.Search.Requests;

namespace SFA.DAS.VacancyServices.Search
{
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
        private readonly VacancyServicesSearchConfiguration _config;

        public TraineeshipSearchClient(VacancyServicesSearchConfiguration config)
            : this(new ElasticSearchFactory(), config)
        {
        }

        internal TraineeshipSearchClient(IElasticSearchFactory elasticSearchFactory, VacancyServicesSearchConfiguration config)
        {
            _elasticSearchFactory = elasticSearchFactory;
            _config = config;
        }

        public IEnumerable<int> GetAllVacancyIds()
        {
            var client = _elasticSearchFactory.GetElasticClient(_config.HostName);

            var scanResults = client.Search<TraineeshipSearchResult>(search => search
                .Index(_config.TraineeshipsIndex)
                .Type(ElasticTypes.Traineeship)
                .From(0)
                .Size(ScrollSize)
                .MatchAll()
                .SearchType(SearchType.Scan)
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
            int geoDistanceSortPosition = -1;

            var client = _elasticSearchFactory.GetElasticClient(_config.HostName);

            var results = client.Search<TraineeshipSearchResult>(s =>
            {
                s.Index(_config.TraineeshipsIndex);
                s.Type(ElasticTypes.Traineeship);
                s.Skip((parameters.PageNumber - 1) * parameters.PageSize);
                s.Take(parameters.PageSize);

                s.TrackScores();

                s.Query(q => GetQuery(parameters, q));

                geoDistanceSortPosition = SetSort(s, parameters);

                return s;
            });

            SetPostSearchValues(parameters, results, geoDistanceSortPosition);

            return results;
        }

        private QueryContainer GetQuery(TraineeshipSearchRequestParameters parameters, QueryDescriptor<TraineeshipSearchResult> q)
        {
            if (!string.IsNullOrEmpty(parameters.VacancyReference))
            {
                return q.Filtered(fq =>
                    fq.Filter(f =>
                        f.Term(t =>
                            t.VacancyReference, parameters.VacancyReference)));
            }

            QueryContainer query = null;
            
            if (parameters.IsLatLongSearch && parameters.SearchRadius != 0)
            {
                var queryClause = q.Filtered(qf => qf.Filter(f => f
                    .GeoDistance(vs => vs
                        .Location, descriptor => descriptor
                            .Location(parameters.Latitude.Value, parameters.Longitude.Value)
                            .Distance(parameters.SearchRadius, GeoUnit.Miles))));
                query &= queryClause;
            }

            return query;
        }

        /// <summary>
        /// Returns the sort position of the GeoDistance sort if applicable.
        /// If the search is not a GeoDistance search then returns -1.
        /// </summary>
        /// <param name="search"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private int SetSort(SearchDescriptor<TraineeshipSearchResult> search, TraineeshipSearchRequestParameters parameters)
        {

            //Rule: Always call TrySortByGeoDistance. This will populate the HitsMetaData.Hits.Sorts so we can display the distance in the results
            switch (parameters.SortType)
            {
                case VacancySearchSortType.RecentlyAdded:

                    search.SortDescending(r => r.PostedDate);
                    search.TrySortByGeoDistance(parameters);

                    return parameters.IsLatLongSearch ? 1 : -1;

                case VacancySearchSortType.Distance:

                    search.TrySortByGeoDistance(parameters);

                    return parameters.IsLatLongSearch ? 0 : -1;

                case VacancySearchSortType.ClosingDate:

                    search.SortAscending(r => r.ClosingDate);
                    search.TrySortByGeoDistance(parameters);

                    return parameters.IsLatLongSearch ? 1 : -1;

                default:

                    search.Sort(sort => sort.OnField("_score").Descending());
                    search.TrySortByGeoDistance(parameters);

                    return parameters.IsLatLongSearch ? 1 : -1;
            }
        }

        private void SetPostSearchValues(TraineeshipSearchRequestParameters searchParameters, ISearchResponse<TraineeshipSearchResult> results, int geoDistanceSortPosition)
        {
            foreach (var result in results.Documents)
            {
                var hitMd = results.HitsMetaData.Hits.First(h => h.Id == result.Id.ToString(CultureInfo.InvariantCulture));

                if (searchParameters.IsLatLongSearch)
                    result.Distance = (double)hitMd.Sorts.ElementAt(geoDistanceSortPosition);

                result.Score = hitMd.Score;
            }
        }
    }
}
