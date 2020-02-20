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

    public class ApprenticeshipSearchClient : IApprenticeshipSearchClient
    {
        private const string SubCategoriesAggregationName = "SubCategoryCodes";

        private const string ScrollIndexConsistencyTime = "5s";
        private const int ScrollSize = 100;
        private const string ScrollTimeout = "5s";

        private readonly IElasticSearchFactory _elasticSearchFactory;
        private readonly ApprenticeshipSearchClientConfiguration _config;
        private readonly SearchFactorConfiguration _searchFactorConfiguration;
        private readonly IEnumerable<string> _keywordExcludedTerms;

        public ApprenticeshipSearchClient(ApprenticeshipSearchClientConfiguration config)
        : this(new ElasticSearchFactory(), config)
        {
        }

        internal ApprenticeshipSearchClient(IElasticSearchFactory elasticSearchFactory, ApprenticeshipSearchClientConfiguration config)
        {
            _elasticSearchFactory = elasticSearchFactory;
            _config = config;
            _searchFactorConfiguration = GetSearchFactorConfiguration();
            _keywordExcludedTerms = new[] {"apprenticeships", "apprenticeship", "traineeship", "traineeships", "trainee"};
        }

        public ApprenticeshipSearchResponse Search(ApprenticeshipSearchRequestParameters searchParameters)
        {
            SanitizeSearchParameters(searchParameters);

            var results = PerformSearch(searchParameters);
           
            var aggregationResults = searchParameters.CalculateSubCategoryAggregations ? 
                GetAggregationResultsFrom(results.Aggs) : 
                null;
            var response = new ApprenticeshipSearchResponse(results.Total, results.Documents, aggregationResults, searchParameters);

            return response;
        }

        public IEnumerable<int> GetAllVacancyIds()
        {
            var client = _elasticSearchFactory.GetElasticClient(_config.HostName);

            var scanResults = client.Search<ApprenticeshipSearchResult>(search => search
                .Index(_config.Index)
                .Type(ElasticTypes.Apprenticeship)
                .From(0)
                .Size(ScrollSize)
                .MatchAll()
                .SearchType(SearchType.Scan)
                .Scroll(ScrollIndexConsistencyTime));

            var vacancyIds = new List<int>();
            var scrollRequest = new ScrollRequest(scanResults.ScrollId, ScrollTimeout);

            while (true)
            {
                var results = client.Scroll<ApprenticeshipSearchResult>(scrollRequest);

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
            var client = _elasticSearchFactory.GetElasticClient(_config.HostName);

            var results = client.Search<ApprenticeshipSearchResult>(s =>
            {
                s.Index(_config.Index);
                s.Type(ElasticTypes.Apprenticeship);
                s.Skip((parameters.PageNumber - 1) * parameters.PageSize);
                s.Take(parameters.PageSize);
                
                s.TrackScores();

                s.Query(q => GetQuery(parameters, q));

                SetSort(s, parameters);

                if(parameters.CalculateSubCategoryAggregations)
                    s.Aggregations(a => a.Terms(SubCategoriesAggregationName, st => st.Field(o => o.SubCategoryCode).Size(0)));

                //Filters to run after the aggregations have been calculated
                if (parameters.SubCategoryCodes != null && parameters.SubCategoryCodes.Any())
                {
                    s.Filter(ff => ff.Terms(f => f.SubCategoryCode, parameters.SubCategoryCodes.Distinct()));
                }

                return s;
            });

            SetHitValuesOnSearchResults(parameters, results);

            return results;
        }

        private QueryContainer GetQuery(ApprenticeshipSearchRequestParameters parameters, QueryDescriptor<ApprenticeshipSearchResult> q)
        {
            if (!string.IsNullOrEmpty(parameters.VacancyReference))
            {
                return q.Filtered(fq =>
                    fq.Filter(f =>
                        f.Term(t =>
                            t.VacancyReference, parameters.VacancyReference)));
            }

            QueryContainer query = null;

            query &= GetKeywordQuery(parameters, q);

            if (parameters.FrameworkLarsCodes.Any() || parameters.StandardLarsCodes.Any())
            {
                var queryClause = q.Terms(apprenticeship => apprenticeship.FrameworkLarsCode, parameters.FrameworkLarsCodes)
                                  || q.Terms(apprenticeship => apprenticeship.StandardLarsCode, parameters.StandardLarsCodes);

                query &= queryClause;
            }

            if (!string.IsNullOrWhiteSpace(parameters.CategoryCode))
            {
                var categoryCodes = new List<string>
                        {
                            parameters.CategoryCode
                        };

                var queryCategory = q.Terms(f => f.CategoryCode, categoryCodes.Distinct());

                query &= queryCategory;
            }

            if (parameters.ExcludeVacancyIds != null && parameters.ExcludeVacancyIds.Any())
            {
                var queryExcludeVacancyIds = !q.Ids(parameters.ExcludeVacancyIds.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                query &= queryExcludeVacancyIds;
            }

            if (parameters.VacancyLocationType != VacancyLocationType.Unknown)
            {
                var queryVacancyLocation = q.Match(m => m.OnField(f => f.VacancyLocationType).Query(parameters.VacancyLocationType.ToString()));

                query &= queryVacancyLocation;
            }

            if (parameters.FromDate.HasValue)
            {
                var queryClause = q.Range(range =>
                    range.OnField(apprenticeship => apprenticeship.PostedDate)
                        .GreaterOrEquals(parameters.FromDate));

                query &= queryClause;
            }

            if (parameters.Ukprn.HasValue)
            {
                var queryClause = q
                    .Match(m => m.OnField(f => f.Ukprn)
                        .Query(parameters.Ukprn.ToString()));
                query &= queryClause;
            }

            if (!string.IsNullOrWhiteSpace(parameters.ApprenticeshipLevel) && parameters.ApprenticeshipLevel != "All")
            {
                var queryClause = q
                    .Match(m => m.OnField(f => f.ApprenticeshipLevel)
                        .Query(parameters.ApprenticeshipLevel));
                query &= queryClause;
            }

            if (parameters.DisabilityConfidentOnly)
            {
                var queryDisabilityConfidentOnly = q
                    .Match(m => m.OnField(f => f.IsDisabilityConfident)
                        .Query(parameters.DisabilityConfidentOnly.ToString()));
                query &= queryDisabilityConfidentOnly;
            }

            if (parameters.CanFilterByGeoDistance)
            {
                var queryClause = q.Filtered(qf => qf.Filter(f => f
                    .GeoDistance(vs => vs
                        .Location, descriptor => descriptor
                            .Location(parameters.Latitude.Value, parameters.Longitude.Value)
                            .Distance(parameters.SearchRadius.Value, GeoUnit.Miles))));

                query &= queryClause;
            }

            return query;
        }

        private QueryContainer GetKeywordQuery(ApprenticeshipSearchRequestParameters parameters, QueryDescriptor<ApprenticeshipSearchResult> q)
        {
            QueryContainer keywordQuery = null;
            if (!string.IsNullOrWhiteSpace(parameters.Keywords)
                && (parameters.SearchField == ApprenticeshipSearchField.All || parameters.SearchField == ApprenticeshipSearchField.JobTitle))
            {
                var queryClause = q.Match(m =>
                {
                    m.OnField(f => f.Title).Query(parameters.Keywords);
                    BuildFieldQuery(m, _searchFactorConfiguration.JobTitleFactors);
                });

                keywordQuery |= queryClause;
            }

            if (!string.IsNullOrWhiteSpace(parameters.Keywords)
                && (parameters.SearchField == ApprenticeshipSearchField.All || parameters.SearchField == ApprenticeshipSearchField.Description))
            {
                var queryClause = q.Match(m =>
                {
                    m.OnField(f => f.Description).Query(parameters.Keywords);
                    BuildFieldQuery(m, _searchFactorConfiguration.DescriptionFactors);
                });

                keywordQuery |= queryClause;
            }

            if (!string.IsNullOrWhiteSpace(parameters.Keywords)
                && (parameters.SearchField == ApprenticeshipSearchField.All || parameters.SearchField == ApprenticeshipSearchField.Employer))
            {
                var queryClause = q.Match(m =>
                {
                    m.OnField(f => f.EmployerName).Query(parameters.Keywords);
                    BuildFieldQuery(m, _searchFactorConfiguration.EmployerFactors);
                });

                keywordQuery |= queryClause;
            }

            return keywordQuery;
        }

        private static void SetSort(SearchDescriptor<ApprenticeshipSearchResult> search, ApprenticeshipSearchRequestParameters parameters)
        {
            switch (parameters.SortType)
            {
                case VacancySearchSortType.RecentlyAdded:
                    search.SortDescending(r => r.PostedDate);
                    search.TrySortByGeoDistance(parameters);
                    search.SortDescending(r => r.VacancyReference);
                    break;
                case VacancySearchSortType.Distance:
                    search.TrySortByGeoDistance(parameters);
                    search.SortDescending(r => r.PostedDate);
                    search.SortDescending(r => r.VacancyReference);
                    break;
                case VacancySearchSortType.ClosingDate:
                    search.SortAscending(r => r.ClosingDate);
                    search.TrySortByGeoDistance(parameters);
                    break;
                case VacancySearchSortType.ExpectedStartDate:
                    search.SortAscending(r => r.StartDate);
                    search.SortAscending(r => r.VacancyReference);
                    search.TrySortByGeoDistance(parameters);
                    break;
                default:
                    search.Sort(sort => sort.OnField("_score").Descending());
                    search.TrySortByGeoDistance(parameters);
                    break;
            }
        }

        private static void BuildFieldQuery(MatchQueryDescriptor<ApprenticeshipSearchResult> queryDescriptor,
            SearchTermFactors searchFactors)
        {
            if (searchFactors.Boost.HasValue)
            {
                queryDescriptor.Boost(searchFactors.Boost.Value);
            }

            if (searchFactors.Fuzziness.HasValue)
            {
                queryDescriptor.Fuzziness(searchFactors.Fuzziness.Value);
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

            if (searchFactors.PhraseProximity.HasValue)
            {
                queryDescriptor.Slop(searchFactors.PhraseProximity.Value);
            }
        }

        private static void SetHitValuesOnSearchResults(ApprenticeshipSearchRequestParameters searchParameters, ISearchResponse<ApprenticeshipSearchResult> results)
        {
            foreach (var result in results.Documents)
            {
                var hitMd = results.HitsMetaData.Hits.First(h => h.Id == result.Id.ToString(CultureInfo.InvariantCulture));

                if(searchParameters.CanSortByGeoDistance)
                    result.Distance = (double)hitMd.Sorts.ElementAt(GetGeoDistanceSortHitPosition(searchParameters));

                result.Score = hitMd.Score;
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

        private static IEnumerable<AggregationResult> GetAggregationResultsFrom(AggregationsHelper aggregations)
        {
            if (aggregations == null)
                return Enumerable.Empty<AggregationResult>();

            var terms = aggregations.Terms(SubCategoriesAggregationName);

            if (terms == null)
            {
                return Enumerable.Empty<AggregationResult>();
            }

            var items = terms.Items.Select(bucket => new AggregationResult
            {
                Code = bucket.Key,
                Count = bucket.DocCount
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
                    MinimumMatch = "2<75%",
                    PhraseProximity = 2
                }
            };
        }
    }
}
