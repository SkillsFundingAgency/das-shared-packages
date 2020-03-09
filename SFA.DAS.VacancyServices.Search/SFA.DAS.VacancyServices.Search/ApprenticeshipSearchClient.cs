namespace SFA.DAS.VacancyServices.Search
{
    using Requests;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Entities;
    using Nest;
    using Responses;
    using SFA.DAS.Elastic;
    using SFA.DAS.NLog.Logger;

    public class ApprenticeshipSearchClient : IApprenticeshipSearchClient
    {
        private const string SubCategoriesAggregationName = "SubCategoryCodes";

        private const string ScrollIndexConsistencyTime = "5s";
        private const int ScrollSize = 100;
        private const string ScrollTimeout = "5s";

        private readonly IElasticClient _elasticClient;
        private readonly SearchFactorConfiguration _searchFactorConfiguration;
        private readonly IEnumerable<string> _keywordExcludedTerms;
        private readonly string _indexName;

        public ApprenticeshipSearchClient(IElasticClientFactory elasticClientFactory, string indexName, ILog logger = null)
        {
            _elasticClient = elasticClientFactory.CreateClient(r => logger?.Debug(r.DebugInformation));
            _indexName = indexName;
            _searchFactorConfiguration = GetSearchFactorConfiguration();
            _keywordExcludedTerms = new[] { "apprenticeships", "apprenticeship", "traineeship", "traineeships", "trainee" };
        }

        public ApprenticeshipSearchResponse Search(ApprenticeshipSearchRequestParameters searchParameters)
        {
            SanitizeSearchParameters(searchParameters);

            var results = PerformSearch(searchParameters);

            var aggregationResults = searchParameters.CalculateSubCategoryAggregations ?
                GetAggregationResultsFrom(results.Aggregations) :
                null;
            var response = new ApprenticeshipSearchResponse(results.Total, results.Documents, aggregationResults, searchParameters);

            return response;
        }

        public IEnumerable<int> GetAllVacancyIds()
        {
            var scanResults = _elasticClient.Search<ApprenticeshipSearchResult>(search => search
                .Index(_indexName)
                .From(0)
                .Size(ScrollSize)
                .MatchAll()
                .Scroll(ScrollIndexConsistencyTime));

            var vacancyIds = new List<int>();
            var scrollRequest = new ScrollRequest(scanResults.ScrollId, ScrollTimeout);

            while (true)
            {
                var results = _elasticClient.Scroll<ApprenticeshipSearchResult>(scrollRequest);

                if (!results.Documents.Any())
                {
                    break;
                }

                vacancyIds.AddRange(results.Documents.Select(each => each.Id));
            }

            return vacancyIds;
        }

        private void SanitizeSearchParameters(ApprenticeshipSearchRequestParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.Keywords))
                return;

            parameters.Keywords = parameters.Keywords.ToLower();

            foreach (var excludedTerm in _keywordExcludedTerms)
            {
                parameters.Keywords = parameters.Keywords.Replace(excludedTerm, "");
            }
        }

        private ISearchResponse<ApprenticeshipSearchResult> PerformSearch(ApprenticeshipSearchRequestParameters parameters)
        {
            var results = _elasticClient.Search<ApprenticeshipSearchResult>(s =>
            {
                s.Index(_indexName);
                s.Skip((parameters.PageNumber - 1) * parameters.PageSize);
                s.Take(parameters.PageSize);

                s.TrackScores();

                s.Query(q => GetQuery(parameters, q));

                SetSort(s, parameters);

                if (parameters.CalculateSubCategoryAggregations)
                    s.Aggregations(a => a.Terms(SubCategoriesAggregationName, st => st.Field(o => o.SubCategoryCode).Size(0)));

                //Filters to run after the aggregations have been calculated
                if (parameters.SubCategoryCodes != null && parameters.SubCategoryCodes.Any())
                {
                    s.PostFilter(ff => ff.Terms(f =>
                        f.Field(g => g.SubCategoryCode)
                        .Terms(parameters.SubCategoryCodes.Distinct())));
                }

                return s;
            });

            SetHitValuesOnSearchResults(parameters, results);

            return results;
        }

        private QueryContainer GetQuery(ApprenticeshipSearchRequestParameters parameters, QueryContainerDescriptor<ApprenticeshipSearchResult> q)
        {
            if (!string.IsNullOrEmpty(parameters.VacancyReference))
            {
                return q.Bool(fq =>
                    fq.Filter(f =>
                        f.Term(t =>
                            t.VacancyReference, parameters.VacancyReference)));
            }

            QueryContainer query = null;

            query &= GetKeywordQuery(parameters, q);

            if (parameters.FrameworkLarsCodes.Any() || parameters.StandardLarsCodes.Any())
            {
                var queryClause = q.Terms(apprenticeship => apprenticeship.Field(f => f.FrameworkLarsCode).Terms(parameters.FrameworkLarsCodes))
                                  || q.Terms(apprenticeship => apprenticeship.Field(f => f.StandardLarsCode).Terms(parameters.StandardLarsCodes));

                query &= queryClause;
            }

            if (!string.IsNullOrWhiteSpace(parameters.CategoryCode))
            {
                var categoryCodes = new List<string>
                        {
                            parameters.CategoryCode
                        };

                var queryCategory = q.Terms(f => f.Field(g => g.CategoryCode).Terms(categoryCodes.Distinct()));

                query &= queryCategory;
            }

            if (parameters.ExcludeVacancyIds != null && parameters.ExcludeVacancyIds.Any())
            {
                var queryExcludeVacancyIds = !q.Ids(i => i.Values(parameters.ExcludeVacancyIds.Select(x => x.ToString(CultureInfo.InvariantCulture))));
                query &= queryExcludeVacancyIds;
            }

            if (parameters.VacancyLocationType != VacancyLocationType.Unknown)
            {
                var queryVacancyLocation = q.Match(m => m.Field(f => f.VacancyLocationType).Query(parameters.VacancyLocationType.ToString()));

                query &= queryVacancyLocation;
            }

            if (parameters.FromDate.HasValue)
            {
                var queryClause = q.DateRange(range =>
                    range.Field(apprenticeship => apprenticeship.PostedDate)
                        .GreaterThanOrEquals(parameters.FromDate));

                query &= queryClause;
            }

            if (parameters.Ukprn.HasValue)
            {
                var queryClause = q
                    .Match(m => m.Field(f => f.Ukprn)
                        .Query(parameters.Ukprn.ToString()));
                query &= queryClause;
            }

            if (!string.IsNullOrWhiteSpace(parameters.ApprenticeshipLevel) && parameters.ApprenticeshipLevel != "All")
            {
                var queryClause = q
                    .Match(m => m.Field(f => f.ApprenticeshipLevel)
                        .Query(parameters.ApprenticeshipLevel));
                query &= queryClause;
            }

            if (parameters.DisabilityConfidentOnly)
            {
                var queryDisabilityConfidentOnly = q
                    .Match(m => m.Field(f => f.IsDisabilityConfident)
                        .Query(parameters.DisabilityConfidentOnly.ToString()));
                query &= queryDisabilityConfidentOnly;
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

        private QueryContainer GetKeywordQuery(ApprenticeshipSearchRequestParameters parameters, QueryContainerDescriptor<ApprenticeshipSearchResult> q)
        {
            QueryContainer keywordQuery = null;
            if (!string.IsNullOrWhiteSpace(parameters.Keywords)
                && (parameters.SearchField == ApprenticeshipSearchField.All || parameters.SearchField == ApprenticeshipSearchField.JobTitle))
            {
                var queryClause = q.Match(m =>
                    BuildFieldQuery(m, _searchFactorConfiguration.JobTitleFactors)
                    .Field(f => f.Title).Query(parameters.Keywords));

                keywordQuery |= queryClause;
            }

            if (!string.IsNullOrWhiteSpace(parameters.Keywords)
                && (parameters.SearchField == ApprenticeshipSearchField.All || parameters.SearchField == ApprenticeshipSearchField.Description))
            {
                var queryClause = q.Match(m =>
                    BuildFieldQuery(m, _searchFactorConfiguration.DescriptionFactors)
                    .Field(f => f.Description).Query(parameters.Keywords)
                );

                keywordQuery |= queryClause;
            }

            if (!string.IsNullOrWhiteSpace(parameters.Keywords)
                && (parameters.SearchField == ApprenticeshipSearchField.All || parameters.SearchField == ApprenticeshipSearchField.Employer))
            {
                var queryClause = q.Match(m =>
                    BuildFieldQuery(m, _searchFactorConfiguration.EmployerFactors)
                    .Field(f => f.EmployerName).Query(parameters.Keywords)
                );

                keywordQuery |= queryClause;
            }

            return keywordQuery;
        }

        private static void SetSort(SearchDescriptor<ApprenticeshipSearchResult> search, ApprenticeshipSearchRequestParameters parameters)
        {
            switch (parameters.SortType)
            {
                case VacancySearchSortType.RecentlyAdded:
                    search.Sort(r => r
                        .TrySortByGeoDistance(parameters)
                        .Descending(s => s.PostedDate)
                        .Descending(s => s.VacancyReference));
                    break;
                case VacancySearchSortType.Distance:
                    search.Sort(s => s
                        .TrySortByGeoDistance(parameters)
                        .Descending(r => r.PostedDate)
                        .Descending(r => r.VacancyReference));
                    break;
                case VacancySearchSortType.ClosingDate:
                    search.Sort(s => s
                        .Ascending(r => r.ClosingDate)
                        .TrySortByGeoDistance(parameters));
                    break;
                case VacancySearchSortType.ExpectedStartDate:
                    search.Sort(s => s
                        .Ascending(r => r.StartDate)
                        .Ascending(r => r.VacancyReference)
                        .TrySortByGeoDistance(parameters));
                    break;
                default:
                    search.Sort(s => s
                        .Descending(SortSpecialField.Score)
                        .TrySortByGeoDistance(parameters));
                    break;
            }
        }

        private static MatchQueryDescriptor<ApprenticeshipSearchResult> BuildFieldQuery(MatchQueryDescriptor<ApprenticeshipSearchResult> queryDescriptor,
            SearchTermFactors searchFactors)
        {
            if (searchFactors.Boost.HasValue)
            {
                queryDescriptor.Boost(searchFactors.Boost.Value);
            }

            if (searchFactors.Fuzziness.HasValue)
            {
                queryDescriptor.Fuzziness(Fuzziness.EditDistance(searchFactors.Fuzziness.Value));
            }

            if (searchFactors.FuzzyPrefix.HasValue)
            {
                queryDescriptor.PrefixLength(searchFactors.FuzzyPrefix.Value);
            }

            if (searchFactors.MatchAllKeywords)
            {
                queryDescriptor.Operator(Operator.And);
            }

            if (!string.IsNullOrWhiteSpace(searchFactors.MinimumMatch))
            {
                queryDescriptor.MinimumShouldMatch(searchFactors.MinimumMatch);
            }

            return queryDescriptor;
        }

        private static void SetHitValuesOnSearchResults(ApprenticeshipSearchRequestParameters searchParameters, ISearchResponse<ApprenticeshipSearchResult> results)
        {
            foreach (var result in results.Documents)
            {
                var hitMd = results.Hits.First(h => h.Id == result.Id.ToString(CultureInfo.InvariantCulture));

                if (searchParameters.CanSortByGeoDistance)
                    result.Distance = (double)hitMd.Sorts.ElementAt(GetGeoDistanceSortHitPosition(searchParameters));

                result.Score = hitMd.Score.GetValueOrDefault(0);
            }
        }

        private static int GetGeoDistanceSortHitPosition(ApprenticeshipSearchRequestParameters searchParameters)
        {
            switch (searchParameters.SortType)
            {
                case VacancySearchSortType.ExpectedStartDate:
                    return 2;
                case VacancySearchSortType.Distance:
                    return 0;
                default:
                    return 1;
            }
        }

        private static IEnumerable<AggregationResult> GetAggregationResultsFrom(AggregateDictionary aggregations)
        {
            if (aggregations == null)
                return Enumerable.Empty<AggregationResult>();

            var terms = aggregations.Terms(SubCategoriesAggregationName);

            if (terms == null)
            {
                return Enumerable.Empty<AggregationResult>();
            }

            var items = terms.Buckets.Select(bucket => new AggregationResult
            {
                Code = bucket.Key,
                Count = bucket.DocCount.GetValueOrDefault(0)
            });

            return items;
        }

        private static SearchFactorConfiguration GetSearchFactorConfiguration()
        {
            return new SearchFactorConfiguration
            {
                JobTitleFactors = new SearchTermFactors
                {
                    Boost = 1.5d,
                    Fuzziness = 1,
                    FuzzyPrefix = 1,
                    MatchAllKeywords = true,
                    MinimumMatch = "100%"
                },
                EmployerFactors = new SearchTermFactors
                {
                    Boost = 5,
                    Fuzziness = 1,
                    FuzzyPrefix = 1,
                    MatchAllKeywords = true,
                    MinimumMatch = "100%"
                },
                DescriptionFactors = new SearchTermFactors
                {
                    Boost = 1.0d,
                    Fuzziness = 1,
                    FuzzyPrefix = 1,
                    MatchAllKeywords = false,
                    MinimumMatch = "2<75%"
                }
            };
        }
    }
}
