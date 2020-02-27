using SFA.DAS.VacancyServices.Search.Requests;

namespace SFA.DAS.VacancyServices.Search.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Elasticsearch.Net;
    using Entities;
    using FluentAssertions;
    using FluentAssertions.Json;
    using Moq;
    using Nest;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using SFA.DAS.Elastic;
    using SFA.DAS.NLog.Logger;

    [TestFixture]
    public class ApprenticeshipSearchClientTests
    {
        #region Vacancy API searches

        [Test]
        public void Search_ShouldSearchByStandardLarsCode()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                PageNumber = 1,
                PageSize = 100,
                VacancyLocationType = VacancyLocationType.NonNational,
                StandardLarsCodes = new List<string>{"123","124"},
                SortType = VacancySearchSortType.RecentlyAdded
            };

            const string expectedJsonQuery = "{\"from\":0,\"size\":100,\"track_scores\":true,\"sort\":[{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"query\":{\"bool\":{\"must\":[{\"terms\":{\"standardLarsCode\":[\"123\",\"124\"]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}}]}}}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchByFrameworkLarsCode()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                PageNumber = 1,
                PageSize = 100,
                VacancyLocationType = VacancyLocationType.NonNational,
                FrameworkLarsCodes = new List<string> { "502","501" },
                SortType = VacancySearchSortType.RecentlyAdded
            };

            const string expectedJsonQuery = "{\"from\":0,\"query\":{\"bool\":{\"must\":[{\"terms\":{\"frameworkLarsCode\":[\"502\",\"501\"]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}}]}},\"size\":100,\"sort\":[{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"track_scores\":true}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchByPostedInLastNumberOfDays()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                PageNumber = 1,
                PageSize = 100,
                VacancyLocationType = VacancyLocationType.NonNational,
                FromDate = DateTime.Parse("2018-12-01"),
                SortType = VacancySearchSortType.RecentlyAdded
            };

            const string expectedJsonQuery = "{\"from\":0,\"size\":100,\"track_scores\":true,\"sort\":[{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"query\":{\"bool\":{\"must\":[{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}},{\"range\":{\"postedDate\":{\"gte\":\"2018-12-01T00:00:00\"}}}]}}}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchByNationwideOnly()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                PageNumber = 1,
                PageSize = 100,
                VacancyLocationType = VacancyLocationType.National,
                SortType = VacancySearchSortType.RecentlyAdded
            };

            const string expectedJsonQuery = "{\"from\":0,\"size\":100,\"track_scores\":true,\"sort\":[{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"query\":{\"match\":{\"vacancyLocationType\":{\"query\":\"National\"}}}}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchByLatAndLongAndSortByDistance()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                PageNumber = 1,
                PageSize = 100,
                VacancyLocationType = VacancyLocationType.NonNational,
                Latitude = 52.4088862063274,
                Longitude = 1.50554768088033,
                SearchRadius = 40.5,
                SortType = VacancySearchSortType.Distance
            };

            const string expectedJsonQuery = "{\"from\":0,\"query\":{\"bool\":{\"filter\":[{\"geo_distance\":{\"distance\":\"40.5mi\",\"location\":{\"lat\":52.4088862063274,\"lon\":1.50554768088033}}}],\"must\":[{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}}]}},\"size\":100,\"sort\":[{\"_geo_distance\":{\"distance_type\":\"arc\",\"unit\":\"mi\",\"location\":[{\"lat\":52.4088862063274,\"lon\":1.50554768088033}]}},{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"track_scores\":true}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSortByAge()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                PageNumber = 1,
                PageSize = 100,
                VacancyLocationType = VacancyLocationType.NonNational,
                StandardLarsCodes = new List<string> { "123", "124" },
                SortType = VacancySearchSortType.RecentlyAdded
            };

            const string expectedJsonQuery = "{\"from\":0,\"size\":100,\"track_scores\":true,\"sort\":[{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"query\":{\"bool\":{\"must\":[{\"terms\":{\"standardLarsCode\":[\"123\",\"124\"]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}}]}}}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSortByExpectedStartDate()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                PageNumber = 1,
                PageSize = 100,
                VacancyLocationType = VacancyLocationType.NonNational,
                StandardLarsCodes = new List<string> { "123", "124" },
                SortType = VacancySearchSortType.ExpectedStartDate
            };

            const string expectedJsonQuery = "{\"from\":0,\"size\":100,\"track_scores\":true,\"sort\":[{\"startDate\":{\"order\":\"asc\"}},{\"vacancyReference\":{\"order\":\"asc\"}}],\"query\":{\"bool\":{\"must\":[{\"terms\":{\"standardLarsCode\":[\"123\",\"124\"]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}}]}}}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchByCombination()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                PageNumber = 2,
                PageSize = 50,
                VacancyLocationType = VacancyLocationType.NonNational,
                StandardLarsCodes = new List<string> { "123", "124" },
                FrameworkLarsCodes = new List<string> { "502", "501" },
                Latitude = 52.4088862063274,
                Longitude = 1.50554768088033,
                Ukprn = 12345678,
                SearchRadius = 40,
                FromDate = DateTime.Parse("2018-11-24"),
                SortType = VacancySearchSortType.ExpectedStartDate
            };

            const string expectedJsonQuery = "{\"from\":50,\"query\":{\"bool\":{\"filter\":[{\"geo_distance\":{\"distance\":\"40mi\",\"location\":{\"lat\":52.4088862063274,\"lon\":1.50554768088033}}}],\"must\":[{\"bool\":{\"should\":[{\"terms\":{\"frameworkLarsCode\":[\"502\",\"501\"]}},{\"terms\":{\"standardLarsCode\":[\"123\",\"124\"]}}]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}},{\"range\":{\"postedDate\":{\"gte\":\"2018-11-24T00:00:00\"}}},{\"match\":{\"ukprn\":{\"query\":\"12345678\"}}}]}},\"size\":50,\"sort\":[{\"startDate\":{\"order\":\"asc\"}},{\"vacancyReference\":{\"order\":\"asc\"}},{\"_geo_distance\":{\"distance_type\":\"arc\",\"location\":[{\"lat\":52.4088862063274,\"lon\":1.50554768088033}],\"unit\":\"mi\"}}],\"track_scores\":true}";
            AssertSearch(parameters, expectedJsonQuery);
        }

        #endregion

        #region FAA searches
        [Test]
        public void Search_ShouldSearchByVacancyReference()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                PageNumber = 1,
                PageSize = 1,
                VacancyReference = "123456789"
            };

            const string expectedJsonQuery = "{\"from\":0,\"query\":{\"bool\":{\"filter\":[{\"term\":{\"vacancyReference\":{\"value\":\"123456789\"}}}]}},\"size\":1,\"sort\":[{\"_score\":{\"order\":\"desc\"}}],\"track_scores\":true}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchBySubCategoryCode()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                PageNumber = 1,
                PageSize = 5,
                SubCategoryCodes = new[] {"sub-code"},
                CalculateSubCategoryAggregations = true
            };

            const string expectedJsonQuery = "{\"aggs\":{\"SubCategoryCodes\":{\"terms\":{\"field\":\"subCategoryCode\",\"size\":0}}},\"from\":0,\"post_filter\":{\"terms\":{\"subCategoryCode\":[\"sub-code\"]}},\"size\":5,\"sort\":[{\"_score\":{\"order\":\"desc\"}}],\"track_scores\":true}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchByLatAndLong()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                ApprenticeshipLevel = "All",
                Keywords = "baker",
                Latitude = 52.4088862063274,
                Longitude = 1.50554768088033,
                PageNumber = 1,
                PageSize = 5,
                SearchField = ApprenticeshipSearchField.All,
                SearchRadius = 40,
                SortType = VacancySearchSortType.Distance,
                VacancyLocationType = VacancyLocationType.NonNational
            };

            const string expectedJsonQuery = "{\"from\":0,\"query\":{\"bool\":{\"filter\":[{\"geo_distance\":{\"distance\":\"40mi\",\"location\":{\"lat\":52.4088862063274,\"lon\":1.50554768088033}}}],\"must\":[{\"bool\":{\"should\":[{\"match\":{\"title\":{\"boost\":1.5,\"fuzziness\":1,\"minimum_should_match\":\"100%\",\"operator\":\"and\",\"prefix_length\":1,\"query\":\"baker\"}}},{\"match\":{\"description\":{\"boost\":1.0,\"fuzziness\":1,\"minimum_should_match\":\"2<75%\",\"prefix_length\":1,\"query\":\"baker\"}}},{\"match\":{\"employerName\":{\"boost\":5.0,\"fuzziness\":1,\"minimum_should_match\":\"100%\",\"operator\":\"and\",\"prefix_length\":1,\"query\":\"baker\"}}}]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}}]}},\"size\":5,\"sort\":[{\"_geo_distance\":{\"distance_type\":\"arc\",\"location\":[{\"lat\":52.4088862063274,\"lon\":1.50554768088033}],\"unit\":\"mi\"}},{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"track_scores\":true}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchByLatAndLongAndDisabilityConfident()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                ApprenticeshipLevel = "All",
                DisabilityConfidentOnly = true,
                Keywords = "baker",
                Latitude = 52.4088862063274,
                Longitude = 1.50554768088033,
                PageNumber = 1,
                PageSize = 5,
                SearchField = ApprenticeshipSearchField.All,
                SearchRadius = 40,
                SortType = VacancySearchSortType.Distance,
                VacancyLocationType = VacancyLocationType.NonNational
            };

            const string expectedJsonQuery = "{\"from\":0,\"query\":{\"bool\":{\"filter\":[{\"geo_distance\":{\"distance\":\"40mi\",\"location\":{\"lat\":52.4088862063274,\"lon\":1.50554768088033}}}],\"must\":[{\"bool\":{\"should\":[{\"match\":{\"title\":{\"boost\":1.5,\"fuzziness\":1,\"minimum_should_match\":\"100%\",\"operator\":\"and\",\"prefix_length\":1,\"query\":\"baker\"}}},{\"match\":{\"description\":{\"boost\":1.0,\"fuzziness\":1,\"minimum_should_match\":\"2<75%\",\"prefix_length\":1,\"query\":\"baker\"}}},{\"match\":{\"employerName\":{\"boost\":5.0,\"fuzziness\":1,\"minimum_should_match\":\"100%\",\"operator\":\"and\",\"prefix_length\":1,\"query\":\"baker\"}}}]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}},{\"match\":{\"isDisabilityConfident\":{\"query\":\"True\"}}}]}},\"size\":5,\"sort\":[{\"_geo_distance\":{\"distance_type\":\"arc\",\"location\":[{\"lat\":52.4088862063274,\"lon\":1.50554768088033}],\"unit\":\"mi\"}},{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"track_scores\":true}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchNationalApprenticeships()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                ApprenticeshipLevel = "All",
                Keywords = "baker",
                PageNumber = 1,
                PageSize = 5,
                SearchField = ApprenticeshipSearchField.All,
                SortType = VacancySearchSortType.ClosingDate,
                VacancyLocationType = VacancyLocationType.National
            };

            const string expectedJsonQuery = "{\"from\":0,\"query\":{\"bool\":{\"must\":[{\"bool\":{\"should\":[{\"match\":{\"title\":{\"boost\":1.5,\"fuzziness\":1,\"minimum_should_match\":\"100%\",\"operator\":\"and\",\"prefix_length\":1,\"query\":\"baker\"}}},{\"match\":{\"description\":{\"boost\":1.0,\"fuzziness\":1,\"minimum_should_match\":\"2<75%\",\"prefix_length\":1,\"query\":\"baker\"}}},{\"match\":{\"employerName\":{\"boost\":5.0,\"fuzziness\":1,\"minimum_should_match\":\"100%\",\"operator\":\"and\",\"prefix_length\":1,\"query\":\"baker\"}}}]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"National\"}}}]}},\"size\":5,\"sort\":[{\"closingDate\":{\"order\":\"asc\"}}],\"track_scores\":true}";

            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchByApprenticeshipLevelIfNotAll()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                ApprenticeshipLevel = "Advanced",
                PageNumber = 1,
                PageSize = 5,
                SearchField = ApprenticeshipSearchField.All,
                SortType = VacancySearchSortType.ClosingDate,
                VacancyLocationType = VacancyLocationType.National
            };

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"closingDate\":{\"order\":\"asc\"}}],\"query\":{\"bool\":{\"must\":[{\"match\":{\"vacancyLocationType\":{\"query\":\"National\"}}},{\"match\":{\"apprenticeshipLevel\":{\"query\":\"Advanced\"}}}]}}}";

            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldIncludeGeoDistanceInSort()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                ApprenticeshipLevel = "Higher",
                Latitude = 52.4088862063274,
                Longitude = 1.50554768088033,
                SearchRadius = null,
                PageNumber = 1,
                PageSize = 5,
                SearchField = ApprenticeshipSearchField.All,
                SortType = VacancySearchSortType.ClosingDate,
                VacancyLocationType = VacancyLocationType.NonNational
            };

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"closingDate\":{\"order\":\"asc\"}},{\"_geo_distance\":{\"distance_type\":\"arc\",\"unit\":\"mi\",\"location\":[{\"lat\":52.4088862063274,\"lon\":1.50554768088033}]}}],\"query\":{\"bool\":{\"must\":[{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}},{\"match\":{\"apprenticeshipLevel\":{\"query\":\"Higher\"}}}]}}}";

            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldExcludeSpecifiedVacancies()
        {
            var parameters = new ApprenticeshipSearchRequestParameters
            {
                ApprenticeshipLevel = "All",
                CategoryCode = "SSAT1.00",
                ExcludeVacancyIds = new[] {123456, 789012},
                Latitude = 52.4088862063274,
                Longitude = -1.50554768088033,
                PageNumber = 1,
                PageSize = 5,
                SearchField = ApprenticeshipSearchField.All,
                SearchRadius = 5,
                SortType = VacancySearchSortType.Distance,
                VacancyLocationType = VacancyLocationType.NonNational
            };

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"_geo_distance\":{\"distance_type\":\"arc\",\"unit\":\"mi\",\"location\":[{\"lat\":52.4088862063274,\"lon\":-1.50554768088033}]}},{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"query\":{\"bool\":{\"filter\":[{\"geo_distance\":{\"distance\":\"5mi\",\"location\":{\"lat\":52.4088862063274,\"lon\":-1.50554768088033}}}],\"must\":[{\"terms\":{\"categoryCode\":[\"SSAT1.00\"]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}}],\"must_not\":[{\"ids\":{\"values\":[\"123456\",\"789012\"]}}]}}}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void GetAllVacancyIds_ShouldScrollResults()
        {
            const string expectedJsonQuery = "{\"from\":0,\"size\":100,\"query\":{\"match_all\":{}}}";

            var searchResponse = new Mock<ISearchResponse<ApprenticeshipSearchResult>>();
            searchResponse.Setup(s => s.ScrollId).Returns("scrollId-100");

            Func<SearchDescriptor<ApprenticeshipSearchResult>, ISearchRequest> actualSearchDescriptorFunc = null;

            var mockClient = new Mock<IElasticClient>();

            mockClient.Setup(c => c.Search(It.IsAny<Func<SearchDescriptor<ApprenticeshipSearchResult>, ISearchRequest>>()))
                .Callback<Func<SearchDescriptor<ApprenticeshipSearchResult>, ISearchRequest>>(d => actualSearchDescriptorFunc = d)
                .Returns(searchResponse.Object);

            var scrollResponse1 = new Mock<ISearchResponse<ApprenticeshipSearchResult>>();
            scrollResponse1.Setup(r => r.Documents)
                .Returns(new List<ApprenticeshipSearchResult>
                {
                    new ApprenticeshipSearchResult{Id = 3},
                    new ApprenticeshipSearchResult{Id = 4}
                });

            var scrollResponse2 = new Mock<ISearchResponse<ApprenticeshipSearchResult>>();
            scrollResponse2.Setup(r => r.Documents)
                .Returns(new List<ApprenticeshipSearchResult>
                {
                    new ApprenticeshipSearchResult{Id = 5},
                    new ApprenticeshipSearchResult{Id = 6}
                });

            var scrollResponse3 = new Mock<ISearchResponse<ApprenticeshipSearchResult>>();
            scrollResponse3.Setup(r => r.Documents)
                .Returns(Enumerable.Empty<ApprenticeshipSearchResult>().ToList());

            mockClient.SetupSequence(c => c.Scroll<ApprenticeshipSearchResult>(It.Is<ScrollRequest>(r => r.ScrollId == "scrollId-100")))
                .Returns(scrollResponse1.Object)
                .Returns(scrollResponse2.Object)
                .Returns(scrollResponse3.Object);

            var mockFactory = new Mock<IElasticClientFactory>();
            mockFactory.Setup(f => f.CreateClient(It.IsAny<Action<IApiCallDetails>>())).Returns(mockClient.Object);

            var mockLogger = new Mock<ILog>();

            var sut = new ApprenticeshipSearchClient(mockFactory.Object, "apprenticeships", mockLogger.Object);

            var actualResponse = sut.GetAllVacancyIds().ToList();

            var baseSearchDescriptor = new SearchDescriptor<ApprenticeshipSearchResult>();
            var query = actualSearchDescriptorFunc(baseSearchDescriptor);

            var elasticClient = new ElasticClient();
            var stream = new MemoryStream();
            elasticClient.RequestResponseSerializer.Serialize(query, stream);
            var actualJsonQuery = System.Text.Encoding.UTF8.GetString(stream.ToArray());

            var actualJsonQueryJToken = JToken.Parse(actualJsonQuery);

            var expectedJsonQueryJToken = JToken.Parse(expectedJsonQuery);

            actualResponse.Count.Should().Be(4);
            actualResponse[0].Should().Be(3);
            actualResponse[1].Should().Be(4);
            actualResponse[2].Should().Be(5);
            actualResponse[3].Should().Be(6);

            actualJsonQueryJToken.Should().BeEquivalentTo(expectedJsonQueryJToken);

            mockClient.Verify(c => c.Scroll<ApprenticeshipSearchResult>(It.IsAny<ScrollRequest>()), Times.Exactly(3));
        }

        #endregion

        private void AssertSearch(ApprenticeshipSearchRequestParameters parameters, string expectedJsonQuery)
        {
            var searchResponse = new Mock<ISearchResponse<ApprenticeshipSearchResult>>();
            searchResponse.Setup(s => s.Total).Returns(0);
            searchResponse.Setup(s => s.Documents).Returns(Enumerable.Empty<ApprenticeshipSearchResult>().ToList());

            Func<SearchDescriptor<ApprenticeshipSearchResult>, ISearchRequest> actualSearchDescriptorFunc = null;

            var mockClient = new Mock<IElasticClient>();

            mockClient.Setup(c => c.Search<ApprenticeshipSearchResult>(It.IsAny<Func<SearchDescriptor<ApprenticeshipSearchResult>, ISearchRequest>>()))
                .Callback<Func<SearchDescriptor<ApprenticeshipSearchResult>, ISearchRequest>>(a => actualSearchDescriptorFunc = a)
                .Returns(searchResponse.Object);

            var mockFactory = new Mock<IElasticClientFactory>();
            mockFactory.Setup(f => f.CreateClient(It.IsAny<Action<IApiCallDetails>>())).Returns(mockClient.Object);

            var mockLogger = new Mock<ILog>();

            var sut = new ApprenticeshipSearchClient(mockFactory.Object, "apprenticeships", mockLogger.Object);

            var response = sut.Search(parameters);

            var baseSearchDescriptor = new SearchDescriptor<ApprenticeshipSearchResult>();
            var query = actualSearchDescriptorFunc(baseSearchDescriptor);

            var elasticClient = new ElasticClient();
            var stream = new MemoryStream();
            elasticClient.RequestResponseSerializer.Serialize(query, stream);
            var actualJsonQuery = System.Text.Encoding.UTF8.GetString(stream.ToArray());

            var actualJsonQueryJToken = JToken.Parse(actualJsonQuery);
            
            var expectedJsonQueryJToken = JToken.Parse(expectedJsonQuery);

            actualJsonQueryJToken.Should().BeEquivalentTo(expectedJsonQueryJToken);
        }
    }
}
