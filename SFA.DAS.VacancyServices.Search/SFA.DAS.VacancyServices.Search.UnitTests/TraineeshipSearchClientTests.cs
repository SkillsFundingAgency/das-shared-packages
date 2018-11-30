using SFA.DAS.VacancyServices.Search.Requests;

namespace SFA.DAS.VacancyServices.Search.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Entities;
    using FluentAssertions;
    using FluentAssertions.Json;
    using Moq;
    using Nest;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    public class TraineeshipSearchClientTests
    {
        [Test]
        public void Search_ShouldSearchByLatAndLong()
        {
            var parameters = new TraineeshipSearchRequestParameters
            {
                Latitude = 52.4173666904458,
                Longitude = -1.88983017452229,
                PageNumber = 1,
                PageSize = 5,
                SearchRadius = 40,
                SortType = VacancySearchSortType.Distance,
            };

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"sort\":[{\"_geo_distance\":{\"location\":\"52.4173666904458, -1.88983017452229\",\"unit\":\"mi\"}}],\"query\":{\"filtered\":{\"filter\":{\"geo_distance\":{\"location\":\"52.4173666904458, -1.88983017452229\",\"distance\":40.0,\"unit\":\"mi\"}}}}}";

            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchByVacancyReference()
        {
            var parameters = new TraineeshipSearchRequestParameters
            {
                Latitude = 52.4173666904458,
                Longitude = -1.88983017452229,
                PageNumber = 1,
                PageSize = 5,
                SearchRadius = 40,
                VacancyReference = "1104004"
            };

            const string expectedJsonQuery = "{\"from\":0,\"size\":5,\"track_scores\":true,\"query\":{\"filtered\":{\"filter\":{\"term\":{\"vacancyReference\":\"1104004\"}}}}}";

            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void GetAllVacancyIds_ShouldScrollResults()
        {
            const string expectedJsonQuery = "{\"from\":0,\"size\":100,\"query\":{\"match_all\":{}}}";

            var searchResponse = new Mock<ISearchResponse<TraineeshipSearchResult>>();
            searchResponse.Setup(s => s.ScrollId).Returns("scrollId-100");

            Func<SearchDescriptor<TraineeshipSearchResult>, SearchDescriptor<TraineeshipSearchResult>> actualSearchDescriptorFunc = null;

            var mockClient = new Mock<IElasticClient>();

            mockClient.Setup(c => c.Search<TraineeshipSearchResult>(It.IsAny<Func<SearchDescriptor<TraineeshipSearchResult>, SearchDescriptor<TraineeshipSearchResult>>>()))
                .Callback<Func<SearchDescriptor<TraineeshipSearchResult>, SearchDescriptor<TraineeshipSearchResult>>>(d => actualSearchDescriptorFunc = d)
                .Returns(searchResponse.Object);

            var scrollResponse1 = new Mock<ISearchResponse<TraineeshipSearchResult>>();
            scrollResponse1.Setup(r => r.Documents)
                .Returns(new List<TraineeshipSearchResult>
                {
                    new TraineeshipSearchResult{Id = 3},
                    new TraineeshipSearchResult{Id = 4}
                });

            var scrollResponse2 = new Mock<ISearchResponse<TraineeshipSearchResult>>();
            scrollResponse2.Setup(r => r.Documents)
                .Returns(new List<TraineeshipSearchResult>
                {
                    new TraineeshipSearchResult{Id = 5},
                    new TraineeshipSearchResult{Id = 6}
                });

            var scrollResponse3 = new Mock<ISearchResponse<TraineeshipSearchResult>>();
            scrollResponse3.Setup(r => r.Documents)
                .Returns(Enumerable.Empty<TraineeshipSearchResult>());

            mockClient.SetupSequence(c => c.Scroll<TraineeshipSearchResult>(It.Is<ScrollRequest>(r => r.ScrollId == "scrollId-100")))
                .Returns(scrollResponse1.Object)
                .Returns(scrollResponse2.Object)
                .Returns(scrollResponse3.Object);

            var factory = new Mock<IElasticSearchFactory>();
            factory.Setup(f => f.GetElasticClient(It.IsAny<string>())).Returns(mockClient.Object);

            var sut = new TraineeshipSearchClient(factory.Object, new VacancyServicesSearchConfiguration());

            var actualResponse = sut.GetAllVacancyIds().ToList();

            var baseSearchDescriptor = new SearchDescriptor<TraineeshipSearchResult>();
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

            mockClient.Verify(c => c.Scroll<TraineeshipSearchResult>(It.IsAny<ScrollRequest>()), Times.Exactly(3));
        }

        private void AssertSearch(TraineeshipSearchRequestParameters parameters, string expectedJsonQuery)
        {
            var searchResponse = new Mock<ISearchResponse<TraineeshipSearchResult>>();
            searchResponse.Setup(s => s.Total).Returns(0);
            searchResponse.Setup(s => s.Documents).Returns(Enumerable.Empty<TraineeshipSearchResult>());

            Func<SearchDescriptor<TraineeshipSearchResult>, SearchDescriptor<TraineeshipSearchResult>> actualSearchDescriptorFunc = null;

            var mockClient = new Mock<IElasticClient>();

            mockClient.Setup(c => c.Search<TraineeshipSearchResult>(It.IsAny<Func<SearchDescriptor<TraineeshipSearchResult>, SearchDescriptor<TraineeshipSearchResult>>>()))
                .Callback<Func<SearchDescriptor<TraineeshipSearchResult>, SearchDescriptor<TraineeshipSearchResult>>>(d => actualSearchDescriptorFunc = d)
                .Returns(searchResponse.Object);

            var factory = new Mock<IElasticSearchFactory>();
            factory.Setup(f => f.GetElasticClient(It.IsAny<string>())).Returns(mockClient.Object);

            var sut = new TraineeshipSearchClient(factory.Object, new VacancyServicesSearchConfiguration());

            var response = sut.Search(parameters);

            var baseSearchDescriptor = new SearchDescriptor<TraineeshipSearchResult>();
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
