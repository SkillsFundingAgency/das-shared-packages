using SFA.DAS.VacancyServices.Search.Requests;

namespace SFA.DAS.VacancyServices.Search
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Elasticsearch.Net;
    using Entities;
    using Nest;
    using Newtonsoft.Json.Linq;
    using Responses;

    public class ApprenticeshipSearchClient : IApprenticeshipSearchClient
    {
        private const string SubCategoriesAggregationName = "SubCategoryCodes";

        private const string ScrollIndexConsistencyTime = "5s";
        private const int ScrollSize = 100;
        private const string ScrollTimeout = "5s";

        private readonly IElasticSearchFactory _elasticSearchFactory;
        private readonly VacancyServicesSearchConfiguration _config;
        private readonly SearchFactorConfiguration _searchFactorConfiguration;
        private readonly IEnumerable<string> _keywordExcludedTerms;

        public ApprenticeshipSearchClient(VacancyServicesSearchConfiguration config)
        : this(new ElasticSearchFactory(), config)
        {
        }

        internal ApprenticeshipSearchClient(IElasticSearchFactory elasticSearchFactory, VacancyServicesSearchConfiguration config)
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

            SetPostSearchValues(searchParameters, results);

            var aggregationResults = GetAggregationResultsFrom(results.Aggs);
            var response = new ApprenticeshipSearchResponse(results.Total, results.Documents, aggregationResults, searchParameters);

            return response;
        }

        public IEnumerable<int> GetAllVacancyIds()
        {
            var client = _elasticSearchFactory.GetElasticClient(_config.HostName);

            var scanResults = client.Search<ApprenticeshipSearchResult>(search => search
                .Index(_config.ApprenticeshipsIndex)
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
            if (!string.IsNullOrEmpty(parameters.Keywords))
            {
                parameters.Keywords = parameters.Keywords.ToLower();

                foreach (var excludedTerm in _keywordExcludedTerms)
                {
                    parameters.Keywords = parameters.Keywords.Replace(excludedTerm, "");
                }
            }
        }

        private ISearchResponse<ApprenticeshipSearchResult> PerformSearch(ApprenticeshipSearchRequestParameters parameters)
        {
            var client = _elasticSearchFactory.GetElasticClient(_config.HostName);

            var result = client.Search<ApprenticeshipSearchResult>(s =>
            {
                s.Index(_config.ApprenticeshipsIndex);
                s.Type(ElasticTypes.Apprenticeship);
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
                    case VacancySearchSortType.Relevancy:

                        s.Fields("_source");
                        if (parameters.IsLatLongSearch == false)
                        {
                            break;
                        }
                        s.ScriptFields(sf =>
                            sf.Add("distance", sfd => sfd
                                .Params(fp =>
                                {
                                    fp.Add(nameof(GeoPoint.lat), parameters.Latitude);
                                    fp.Add(nameof(GeoPoint.lon), parameters.Longitude);
                                    return fp;
                                })
                                .Script($"doc['location'].arcDistanceInMiles({nameof(GeoPoint.lat)}, {nameof(GeoPoint.lon)})")));
                        break;
                }

                s.Aggregations(a => a.Terms(SubCategoriesAggregationName, st => st.Field(o => o.SubCategoryCode).Size(0)));

                if (parameters.SubCategoryCodes != null && parameters.SubCategoryCodes.Any())
                {
                    var subCategoryCodes = new List<string>();

                    subCategoryCodes.AddRange(parameters.SubCategoryCodes);

                    s.Filter(ff => ff.Terms(f => f.SubCategoryCode, subCategoryCodes.Distinct()));
                }

                return s;
            });

            return result;
        }

        private QueryContainer GetQuery(ApprenticeshipSearchRequestParameters parameters, QueryDescriptor<ApprenticeshipSearchResult> q)
        {
            QueryContainer query = null;

            if (!string.IsNullOrWhiteSpace(parameters.Keywords)
                && (parameters.SearchField == ApprenticeshipSearchField.All || parameters.SearchField == ApprenticeshipSearchField.JobTitle))
            {
                var queryClause = q.Match(m =>
                {
                    m.OnField(f => f.Title).Query(parameters.Keywords);
                    BuildFieldQuery(m, _searchFactorConfiguration.JobTitleFactors);
                });

                query = BuildContainer(null, queryClause);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Keywords)
                && (parameters.SearchField == ApprenticeshipSearchField.All || parameters.SearchField == ApprenticeshipSearchField.Description))
            {
                var queryClause = q.Match(m =>
                {
                    m.OnField(f => f.Description).Query(parameters.Keywords);
                    BuildFieldQuery(m, _searchFactorConfiguration.DescriptionFactors);
                });
                query = BuildContainer(query, queryClause);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Keywords)
                && (parameters.SearchField == ApprenticeshipSearchField.All || parameters.SearchField == ApprenticeshipSearchField.Employer))
            {
                var exactMatchClause = q.Match(m =>
                {
                    m.OnField(f => f.EmployerName).Query(parameters.Keywords);
                    BuildFieldQuery(m, _searchFactorConfiguration.EmployerFactors);
                });
                query = BuildContainer(query, exactMatchClause);
            }

            if (!string.IsNullOrWhiteSpace(parameters.CategoryCode))
            {
                var categoryCodes = new List<string>
                        {
                            parameters.CategoryCode
                        };

                var queryCategory = q.Terms(f => f.CategoryCode, categoryCodes.Distinct());

                query = query && queryCategory;
            }

            if (parameters.ExcludeVacancyIds != null && parameters.ExcludeVacancyIds.Any())
            {
                var queryExcludeVacancyIds = !q.Ids(parameters.ExcludeVacancyIds.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                query = query && queryExcludeVacancyIds;
            }

            if (parameters.VacancyLocationType != VacancyLocationType.Unknown)
            {
                var queryVacancyLocation = q.Match(m => m.OnField(f => f.VacancyLocationType).Query(parameters.VacancyLocationType.ToString()));

                query = query && queryVacancyLocation;
            }

            if (!string.IsNullOrWhiteSpace(parameters.ApprenticeshipLevel) && parameters.ApprenticeshipLevel != "All")
            {
                var queryClause = q
                    .Match(m => m.OnField(f => f.ApprenticeshipLevel)
                        .Query(parameters.ApprenticeshipLevel));
                query = query && queryClause;
            }

            if (parameters.DisabilityConfidentOnly)
            {
                var queryDisabilityConfidentOnly = q.Match(m => m.OnField(f => f.IsDisabilityConfident)
                                                                 .Query(parameters.DisabilityConfidentOnly.ToString()));
                query = query && queryDisabilityConfidentOnly;
            }

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

        private QueryContainer BuildContainer(QueryContainer queryContainer, QueryContainer queryClause)
        {
            if (queryContainer == null)
            {
                queryContainer = queryClause;
            }
            else
            {
                queryContainer |= queryClause;
            }

            return queryContainer;
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

        private void SetPostSearchValues(ApprenticeshipSearchRequestParameters searchParameters, ISearchResponse<ApprenticeshipSearchResult> results)
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
                    else
                    {
                        var array = hitMd.Fields.FieldValues<JArray>("distance");
                        var value = array[0];

                        result.Distance = double.Parse(value.ToString());
                    }
                }

                result.Score = hitMd.Score;
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
