﻿namespace SFA.DAS.VacancyServices.Search
{
    using Requests;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Entities;
    using Nest;
    using Responses;
    using SFA.DAS.NLog.Logger;
    using SFA.DAS.Elastic;
    using System;

    public class TraineeshipSearchClient : ITraineeshipSearchClient
    {
        private const string ScrollIndexConsistencyTime = "5s";
        private const int ScrollSize = 100;
        private const string ScrollTimeout = "5s";

        private readonly IElasticClient _elasticClient;
        private readonly string _indexName;
        private readonly ILog _logger;

        public TraineeshipSearchClient(IElasticClientFactory elasticClientFactory, string indexName, ILog logger = null)
        {
            _elasticClient = elasticClientFactory.CreateClient(r => logger?.Debug(r.DebugInformation));
            _indexName = indexName;
            _logger = logger;
        }

        public IEnumerable<int> GetAllVacancyIds()
        {
            var scanResults = _elasticClient.Search<TraineeshipSearchResult>(search => search
                .Index(_indexName)
                .From(0)
                .Size(ScrollSize)
                .MatchAll()
                .Scroll(ScrollIndexConsistencyTime));

            var vacancyIds = new List<int>();
            var scrollRequest = new ScrollRequest(scanResults.ScrollId, ScrollTimeout);

            while (true)
            {
                var results = _elasticClient.Scroll<TraineeshipSearchResult>(scrollRequest);

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
            var results = _elasticClient.Search<TraineeshipSearchResult>(s =>
            {
                s.Index(_indexName);
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
                // Nest package won't allow a boolean directly and has to be a string
                // Elastic will throw if its not lower case
                // As we specifically only add this when this statement is true, explicit value is passed.
                var queryDisabilityConfidentOnly = q
                    .Match(m => m.Field(f => f.IsDisabilityConfident)
                        .Query("true"));

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
                var geoQueryClause = q.Bool(qf => qf.Filter(f => f
                    .GeoDistance(vs => vs
                        .Field(field => field.Location)
                        .Location(parameters.Latitude.Value, parameters.Longitude.Value)
                        .Distance(parameters.SearchRadius.Value, DistanceUnit.Miles))));

                query &= geoQueryClause;
            }

            return query;
        }

        private static void SetSort(SearchDescriptor<TraineeshipSearchResult> search, TraineeshipSearchRequestParameters parameters)
        {
            switch (parameters.SortType)
            {
                case VacancySearchSortType.RecentlyAdded:
                    search.Sort(s => s
                        .Descending(r => r.PostedDate)
                        .TrySortByGeoDistance(parameters));
                    break;
                case VacancySearchSortType.Distance:
                    search.Sort(s => s
                        .TrySortByGeoDistance(parameters));
                    break;
                case VacancySearchSortType.ClosingDate:
                    search.Sort(s => s
                        .Ascending(r => r.ClosingDate)
                        .TrySortByGeoDistance(parameters));
                    break;
                default:
                    search.Sort(s => s
                        .Descending(SortSpecialField.Score)
                        .TrySortByGeoDistance(parameters));
                    break;
            }
        }

        private void SetHitValuesOnSearchResults(TraineeshipSearchRequestParameters searchParameters, ISearchResponse<TraineeshipSearchResult> results)
        {
            foreach (var result in results.Documents)
            {
                var hitMd = results.Hits.First(h => h.Id == result.Id.ToString(CultureInfo.InvariantCulture));

                if (searchParameters.CanSortByGeoDistance)
                {
                    try
                    {
                        var distance = hitMd.Sorts.ElementAt(GetGeoDistanceSortHitPosition(searchParameters));
                        result.Distance = Convert.ToDouble(distance);
                    }
                    catch (Exception e)
                    {
                        _logger?.Error(e, "Error converting distance sort value from Elastic Result Set");
                        result.Distance = 0;
                    }
                }

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
