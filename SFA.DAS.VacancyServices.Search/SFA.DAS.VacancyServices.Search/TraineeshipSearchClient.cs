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

            SetPostSearchValues(searchParameters, results);
            
            var response = new TraineeshipSearchResponse(results.Total, results.Documents, searchParameters);

            return response;
        }
        
        private ISearchResponse<TraineeshipSearchResult> PerformSearch(TraineeshipSearchRequestParameters parameters)
        {
            var client = _elasticSearchFactory.GetElasticClient(_config.HostName);

            var result = client.Search<TraineeshipSearchResult>(s =>
            {
                s.Index(_config.TraineeshipsIndex);
                s.Type(ElasticTypes.Traineeship);
                s.Skip((parameters.PageNumber - 1) * parameters.PageSize);
                s.Take(parameters.PageSize);

                s.TrackScores();

                s.Query(q =>
                {
                    if(string.IsNullOrEmpty(parameters.VacancyReference))
                        return GetQuery(parameters, q);

                    return q.Filtered(sl =>
                        sl.Filter(fs =>
                            fs.Term(f =>
                                f.VacancyReference, parameters.VacancyReference)));
                });

                switch (parameters.SortType)
                {
                    case VacancySearchSortType.RecentlyAdded:

                        s.Sort(v => v.OnField(f => f.PostedDate).Descending());
                        s.TrySortByGeoDistance(parameters);

                        break;
                    case VacancySearchSortType.Distance:

                        s.TrySortByGeoDistance(parameters);

                        break;
                    case VacancySearchSortType.ClosingDate:

                        s.Sort(v => v.OnField(f => f.ClosingDate).Ascending());
                        s.TrySortByGeoDistance(parameters);
                        
                        break;
                }

                return s;
            });

            return result;
        }

        private QueryContainer GetQuery(TraineeshipSearchRequestParameters parameters, QueryDescriptor<TraineeshipSearchResult> q)
        {
            QueryContainer query = null;
            
            if (parameters.IsLatLongSearch && parameters.SearchRadius != 0)
            {
                var queryClause = q.Filtered(qf => qf.Filter(f => f
                    .GeoDistance(vs => vs
                        .Location, descriptor => descriptor
                            .Location(parameters.Latitude.Value, parameters.Longitude.Value)
                            .Distance(parameters.SearchRadius, GeoUnit.Miles))));
                query = query && queryClause;
            }

            return query;
        }

        private void SetPostSearchValues(TraineeshipSearchRequestParameters searchParameters, ISearchResponse<TraineeshipSearchResult> results)
        {
            foreach (var result in results.Documents)
            {
                var hitMd = results.HitsMetaData.Hits.First(h => h.Id == result.Id.ToString(CultureInfo.InvariantCulture));

                if (searchParameters.IsLatLongSearch)
                {
                    if (searchParameters.SortType == VacancySearchSortType.ClosingDate ||
                        searchParameters.SortType == VacancySearchSortType.Distance ||
                        searchParameters.SortType == VacancySearchSortType.RecentlyAdded)
                    {
                        result.Distance = double.Parse(hitMd.Sorts.Skip(hitMd.Sorts.Count() - 1).First().ToString());
                    }
                }

                result.Score = hitMd.Score;
            }
        }
    }
}
