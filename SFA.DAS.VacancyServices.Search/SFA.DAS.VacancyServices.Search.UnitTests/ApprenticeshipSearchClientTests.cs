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

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"_geo_distance\":{\"location\":\"52.4088862063274, 1.50554768088033\",\"unit\":\"mi\"}}],\"aggs\":{\"SubCategoryCodes\":{\"terms\":{\"field\":\"subCategoryCode\",\"size\":0}}},\"query\":{\"bool\":{\"must\":[{\"bool\":{\"should\":[{\"match\":{\"title\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"boost\":1.5,\"minimum_should_match\":\"100%\",\"operator\":\"and\"}}},{\"match\":{\"description\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"slop\":2,\"boost\":1.0,\"minimum_should_match\":\"2<75%\"}}},{\"match\":{\"employerName\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"boost\":5.0,\"minimum_should_match\":\"100%\",\"operator\":\"and\"}}}]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}},{\"filtered\":{\"filter\":{\"geo_distance\":{\"location\":\"52.4088862063274, 1.50554768088033\",\"distance\":40.0,\"unit\":\"mi\"}}}}]}}}";
            
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

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"closingDate\":{\"order\":\"asc\"}}],\"aggs\":{\"SubCategoryCodes\":{\"terms\":{\"field\":\"subCategoryCode\",\"size\":0}}},\"query\":{\"bool\":{\"must\":[{\"bool\":{\"should\":[{\"match\":{\"title\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"boost\":1.5,\"minimum_should_match\":\"100%\",\"operator\":\"and\"}}},{\"match\":{\"description\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"slop\":2,\"boost\":1.0,\"minimum_should_match\":\"2<75%\"}}},{\"match\":{\"employerName\":{\"query\":\"baker\",\"fuzziness\":1.0,\"prefix_length\":1,\"boost\":5.0,\"minimum_should_match\":\"100%\",\"operator\":\"and\"}}}]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"National\"}}}]}}}";

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

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"closingDate\":{\"order\":\"asc\"}}],\"aggs\":{\"SubCategoryCodes\":{\"terms\":{\"field\":\"subCategoryCode\",\"size\":0}}},\"query\":{\"bool\":{\"must\":[{\"match\":{\"vacancyLocationType\":{\"query\":\"National\"}}},{\"match\":{\"apprenticeshipLevel\":{\"query\":\"Advanced\"}}}]}}}";

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

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"_geo_distance\":{\"location\":\"52.4088862063274, -1.50554768088033\",\"unit\":\"mi\"}}],\"aggs\":{\"SubCategoryCodes\":{\"terms\":{\"field\":\"subCategoryCode\",\"size\":0}}},\"query\":{\"bool\":{\"must\":[{\"terms\":{\"categoryCode\":[\"SSAT1.00\"]}},{\"match\":{\"vacancyLocationType\":{\"query\":\"NonNational\"}}},{\"filtered\":{\"filter\":{\"geo_distance\":{\"location\":\"52.4088862063274, -1.50554768088033\",\"distance\":5.0,\"unit\":\"mi\"}}}}],\"must_not\":[{\"ids\":{\"values\":[\"123456\",\"789012\"]}}]}}}";

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

            var sut = new ApprenticeshipSearchClient(factory.Object, new VacancyServicesSearchConfiguration());

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

            var sut = new ApprenticeshipSearchClient(factory.Object, new VacancyServicesSearchConfiguration());

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
