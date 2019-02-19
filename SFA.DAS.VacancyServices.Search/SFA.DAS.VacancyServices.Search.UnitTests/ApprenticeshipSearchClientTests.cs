using SFA.DAS.VacancyServices.Search.Requests;

namespace SFA.DAS.VacancyServices.Search.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using FluentAssertions;
    using FluentAssertions.Json;
    using Moq;
    using Nest;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

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

            const string expectedJsonQuery = "{\"from\":0,\"size\":100,\"track_scores\":true,\"sort\":[{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"query\":{\"bool\":{\"must\":[{\"terms\":{\"frameworkLarsCode\":[\"502\",\"501\"]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}}]}}}";
            
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

            const string expectedJsonQuery = "{\"from\":0,\"size\":100,\"track_scores\":true,\"sort\":[{\"_geo_distance\":{\"location\":\"52.4088862063274, 1.50554768088033\",\"unit\":\"mi\"}},{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"query\":{\"bool\":{\"must\":[{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}},{\"filtered\":{\"filter\":{\"geo_distance\":{\"location\":\"52.4088862063274, 1.50554768088033\",\"distance\":40.5,\"unit\":\"mi\"}}}}]}}}";
            
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
                SearchRadius = 40,
                FromDate = DateTime.Parse("2018-11-24"),
                SortType = VacancySearchSortType.ExpectedStartDate
            };

            const string expectedJsonQuery = "{\"from\":50,\"size\":50,\"track_scores\":true,\"sort\":[{\"startDate\":{\"order\":\"asc\"}},{\"vacancyReference\":{\"order\":\"asc\"}},{\"_geo_distance\":{\"location\":\"52.4088862063274, 1.50554768088033\",\"unit\":\"mi\"}}],\"query\":{\"bool\":{\"must\":[{\"bool\":{\"should\":[{\"terms\":{\"frameworkLarsCode\":[\"502\",\"501\"]}},{\"terms\":{\"standardLarsCode\":[\"123\",\"124\"]}}]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}},{\"range\":{\"postedDate\":{\"gte\":\"2018-11-24T00:00:00\"}}},{\"filtered\":{\"filter\":{\"geo_distance\":{\"location\":\"52.4088862063274, 1.50554768088033\",\"distance\":40.0,\"unit\":\"mi\"}}}}]}}}";
            
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

            const string expectedJsonQuery = "{\"from\":0,\"size\":1,\"track_scores\":true,\"sort\":[{\"_score\":{\"order\":\"desc\"}}],\"query\":{\"filtered\":{\"filter\":{\"term\":{\"vacancyReference\":\"123456789\"}}}}}";
            
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

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"_score\":{\"order\":\"desc\"}}],\"aggs\":{\"SubCategoryCodes\":{\"terms\":{\"field\":\"subCategoryCode\",\"size\":0}}},\"filter\":{\"terms\":{\"subCategoryCode\":[\"sub-code\"]}}}";
            
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

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"_geo_distance\":{\"location\":\"52.4088862063274, 1.50554768088033\",\"unit\":\"mi\"}},{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"query\":{\"bool\":{\"must\":[{\"bool\":{\"should\":[{\"match\":{\"title\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"boost\":1.5,\"minimum_should_match\":\"100%\",\"operator\":\"and\"}}},{\"match\":{\"description\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"slop\":2,\"boost\":1.0,\"minimum_should_match\":\"2<75%\"}}},{\"match\":{\"employerName\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"boost\":5.0,\"minimum_should_match\":\"100%\",\"operator\":\"and\"}}}]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}},{\"filtered\":{\"filter\":{\"geo_distance\":{\"location\":\"52.4088862063274, 1.50554768088033\",\"distance\":40.0,\"unit\":\"mi\"}}}}]}}}";
            
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

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"_geo_distance\":{\"location\":\"52.4088862063274, 1.50554768088033\",\"unit\":\"mi\"}},{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"query\":{\"bool\":{\"must\":[{\"bool\":{\"should\":[{\"match\":{\"title\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"boost\":1.5,\"minimum_should_match\":\"100%\",\"operator\":\"and\"}}},{\"match\":{\"description\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"slop\":2,\"boost\":1.0,\"minimum_should_match\":\"2<75%\"}}},{\"match\":{\"employerName\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"boost\":5.0,\"minimum_should_match\":\"100%\",\"operator\":\"and\"}}}]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}},{\"match\":{\"isDisabilityConfident\":{\"query\":\"True\"}}},{\"filtered\":{\"filter\":{\"geo_distance\":{\"location\":\"52.4088862063274, 1.50554768088033\",\"distance\":40.0,\"unit\":\"mi\"}}}}]}}}";
            
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
                SearchRadius = 40,
                SortType = VacancySearchSortType.ClosingDate,
                VacancyLocationType = VacancyLocationType.National
            };

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"closingDate\":{\"order\":\"asc\"}}],\"query\":{\"bool\":{\"must\":[{\"bool\":{\"should\":[{\"match\":{\"title\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"boost\":1.5,\"minimum_should_match\":\"100%\",\"operator\":\"and\"}}},{\"match\":{\"description\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"slop\":2,\"boost\":1.0,\"minimum_should_match\":\"2<75%\"}}},{\"match\":{\"employerName\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"boost\":5.0,\"minimum_should_match\":\"100%\",\"operator\":\"and\"}}}]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"National\"}}}]}}}";

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

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"_geo_distance\":{\"location\":\"52.4088862063274, -1.50554768088033\",\"unit\":\"mi\"}},{\"postedDate\":{\"order\":\"desc\"}},{\"vacancyReference\":{\"order\":\"desc\"}}],\"query\":{\"bool\":{\"must\":[{\"terms\":{\"categoryCode\":[\"SSAT1.00\"]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}},{\"filtered\":{\"filter\":{\"geo_distance\":{\"location\":\"52.4088862063274, -1.50554768088033\",\"distance\":5.0,\"unit\":\"mi\"}}}}],\"must_not\":[{\"ids\":{\"values\":[\"123456\",\"789012\"]}}]}}}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void GetAllVacancyIds_ShouldScrollResults()
        {
            const string expectedJsonQuery = "{\"from\":0,\"size\":100,\"query\":{\"match_all\":{}}}";

            var searchResponse = new Mock<ISearchResponse<ApprenticeshipSearchResult>>();
            searchResponse.Setup(s => s.ScrollId).Returns("scrollId-100");

            Func<SearchDescriptor<ApprenticeshipSearchResult>, SearchDescriptor<ApprenticeshipSearchResult>> actualSearchDescriptorFunc = null;

            var mockClient = new Mock<IElasticClient>();

            mockClient.Setup(c => c.Search<ApprenticeshipSearchResult>(It.IsAny<Func<SearchDescriptor<ApprenticeshipSearchResult>, SearchDescriptor<ApprenticeshipSearchResult>>>()))
                .Callback<Func<SearchDescriptor<ApprenticeshipSearchResult>, SearchDescriptor<ApprenticeshipSearchResult>>>(d => actualSearchDescriptorFunc = d)
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
                .Returns(Enumerable.Empty<ApprenticeshipSearchResult>());

            mockClient.SetupSequence(c => c.Scroll<ApprenticeshipSearchResult>(It.Is<ScrollRequest>(r => r.ScrollId == "scrollId-100")))
                .Returns(scrollResponse1.Object)
                .Returns(scrollResponse2.Object)
                .Returns(scrollResponse3.Object);

            var factory = new Mock<IElasticSearchFactory>();
            factory.Setup(f => f.GetElasticClient(It.IsAny<string>())).Returns(mockClient.Object);

            var sut = new ApprenticeshipSearchClient(factory.Object, new ApprenticeshipSearchClientConfiguration());

            var actualResponse = sut.GetAllVacancyIds().ToList();

            var baseSearchDescriptor = new SearchDescriptor<ApprenticeshipSearchResult>();
            var query = actualSearchDescriptorFunc(baseSearchDescriptor);

            var elasticClient = new ElasticClient();

            var actualJsonQueryBytes = elasticClient.Serializer.Serialize(query);
            var actualJsonQuery = System.Text.Encoding.UTF8.GetString(actualJsonQueryBytes.ToArray());

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
            searchResponse.Setup(s => s.Documents).Returns(Enumerable.Empty<ApprenticeshipSearchResult>());

            Func<SearchDescriptor<ApprenticeshipSearchResult>, SearchDescriptor<ApprenticeshipSearchResult>> actualSearchDescriptorFunc = null;

            var mockClient = new Mock<IElasticClient>();

            mockClient.Setup(c => c.Search<ApprenticeshipSearchResult>(It.IsAny<Func<SearchDescriptor<ApprenticeshipSearchResult>, SearchDescriptor<ApprenticeshipSearchResult>>>()))
                .Callback<Func<SearchDescriptor<ApprenticeshipSearchResult>, SearchDescriptor<ApprenticeshipSearchResult>>>(d => actualSearchDescriptorFunc = d)
                .Returns(searchResponse.Object);

            var factory = new Mock<IElasticSearchFactory>();
            factory.Setup(f => f.GetElasticClient(It.IsAny<string>())).Returns(mockClient.Object);

            var sut = new ApprenticeshipSearchClient(factory.Object, new ApprenticeshipSearchClientConfiguration());

            var response = sut.Search(parameters);

            var baseSearchDescriptor = new SearchDescriptor<ApprenticeshipSearchResult>();
            var query = actualSearchDescriptorFunc(baseSearchDescriptor);

            var elasticClient = new ElasticClient();

            var actualJsonQueryBytes = elasticClient.Serializer.Serialize(query);
            var actualJsonQuery = System.Text.Encoding.UTF8.GetString(actualJsonQueryBytes.ToArray());

            var actualJsonQueryJToken = JToken.Parse(actualJsonQuery);
            
            var expectedJsonQueryJToken = JToken.Parse(expectedJsonQuery);

            actualJsonQueryJToken.Should().BeEquivalentTo(expectedJsonQueryJToken);
        }
    }
}
