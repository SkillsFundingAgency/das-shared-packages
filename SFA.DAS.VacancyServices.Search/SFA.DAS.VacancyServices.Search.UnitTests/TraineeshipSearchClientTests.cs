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
                SearchRadius = 40.5d,
                SortType = VacancySearchSortType.Distance,
            };

            const string expectedJsonQuery = "{\"from\":0,\"query\":{\"bool\":{\"filter\":[{\"geo_distance\":{\"distance\":\"40.5mi\",\"location\":{\"lat\":52.4173666904458,\"lon\":-1.88983017452229}}}]}},\"size\":5,\"sort\":[{\"_geo_distance\":{\"distance_type\":\"arc\",\"unit\":\"mi\",\"location\":[{\"lat\":52.4173666904458,\"lon\":-1.88983017452229}]}}],\"track_scores\":true}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchByLatAndLongAndDisabilityConfident()
        {
            var parameters = new TraineeshipSearchRequestParameters
            {
                DisabilityConfidentOnly = true,
                Latitude = 52.4173666904458,
                Longitude = -1.88983017452229,
                PageNumber = 1,
                PageSize = 5,
                SearchRadius = 40.5d,
                SortType = VacancySearchSortType.Distance,
            };

            const string expectedJsonQuery = "{\"from\":0,\"query\":{\"bool\":{\"filter\":[{\"geo_distance\":{\"distance\":\"40.5mi\",\"location\":{\"lat\":52.4173666904458,\"lon\":-1.88983017452229}}}],\"must\":[{\"match\":{\"isDisabilityConfident\":{\"query\":\"True\"}}}]}},\"size\":5,\"sort\":[{\"_geo_distance\":{\"distance_type\":\"arc\",\"unit\":\"mi\",\"location\":[{\"lat\":52.4173666904458,\"lon\":-1.88983017452229}]}}],\"track_scores\":true}";

            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldSearchByCombination()
        {
            var parameters = new TraineeshipSearchRequestParameters
            {
                DisabilityConfidentOnly = true,
                Latitude = 52.4173666904458,
                Longitude = -1.88983017452229,
                Ukprn = 12345678,
                PageNumber = 1,
                PageSize = 5,
                SearchRadius = 40.5d,
                SortType = VacancySearchSortType.Distance,
            };

            const string expectedJsonQuery = "{\"from\":0,\"query\":{\"bool\":{\"filter\":[{\"geo_distance\":{\"distance\":\"40.5mi\",\"location\":{\"lat\":52.4173666904458,\"lon\":-1.88983017452229}}}],\"must\":[{\"match\":{\"isDisabilityConfident\":{\"query\":\"True\"}}},{\"match\":{\"ukprn\":{\"query\":\"12345678\"}}}]}},\"size\":5,\"sort\":[{\"_geo_distance\":{\"distance_type\":\"arc\",\"location\":[{\"lat\":52.4173666904458,\"lon\":-1.88983017452229}],\"unit\":\"mi\"}}],\"track_scores\":true}";

            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void Search_ShouldIncludeGeoDistanceInSort()
        {
            var parameters = new TraineeshipSearchRequestParameters
            {
                DisabilityConfidentOnly = true,
                Latitude = 52.4173666904458,
                Longitude = -1.88983017452229,
                PageNumber = 1,
                PageSize = 5,
                SearchRadius = null,
                SortType = VacancySearchSortType.RecentlyAdded,
            };

            const string expectedJsonQuery = "{\"from\":0,\"query\":{\"match\":{\"isDisabilityConfident\":{\"query\":\"True\"}}},\"size\":5,\"sort\":[{\"postedDate\":{\"order\":\"desc\"}},{\"_geo_distance\":{\"distance_type\":\"arc\",\"unit\":\"mi\",\"location\":[{\"lat\":52.4173666904458,\"lon\":-1.88983017452229}]}}],\"track_scores\":true}";

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

            const string expectedJsonQuery = "{\"from\":0,\"query\":{\"bool\":{\"filter\":[{\"term\":{\"vacancyReference\":{\"value\":\"1104004\"}}}]}},\"size\":5,\"sort\":[{\"_score\":{\"order\":\"desc\"}},{\"_geo_distance\":{\"distance_type\":\"arc\",\"unit\":\"mi\",\"location\":[{\"lat\":52.4173666904458,\"lon\":-1.88983017452229}]}}],\"track_scores\":true}";
            
            AssertSearch(parameters, expectedJsonQuery);
        }

        [Test]
        public void GetAllVacancyIds_ShouldScrollResults()
        {
            const string expectedJsonQuery = "{\"from\":0,\"size\":100,\"query\":{\"match_all\":{}}}";

            var searchResponse = new Mock<ISearchResponse<TraineeshipSearchResult>>();
            searchResponse.Setup(s => s.ScrollId).Returns("scrollId-100");

            Func<SearchDescriptor<TraineeshipSearchResult>, ISearchRequest> actualSearchDescriptorFunc = null;

            var mockClient = new Mock<IElasticClient>();

            mockClient.Setup(c => c.Search<TraineeshipSearchResult>(It.IsAny<Func<SearchDescriptor<TraineeshipSearchResult>, ISearchRequest>>()))
                .Callback<Func<SearchDescriptor<TraineeshipSearchResult>, ISearchRequest>>(d => actualSearchDescriptorFunc = d)
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
                .Returns(Enumerable.Empty<TraineeshipSearchResult>().ToList());

            mockClient.SetupSequence(c => c.Scroll<TraineeshipSearchResult>(It.Is<ScrollRequest>(r => r.ScrollId == "scrollId-100")))
                .Returns(scrollResponse1.Object)
                .Returns(scrollResponse2.Object)
                .Returns(scrollResponse3.Object);

            var mockFactory = new Mock<IElasticClientFactory>();
            mockFactory.Setup(f => f.CreateClient(It.IsAny<Action<IApiCallDetails>>())).Returns(mockClient.Object);

            var mockLogger = new Mock<ILog>();

            var sut = new TraineeshipSearchClient(mockFactory.Object, "traineeships", mockLogger.Object);

            var actualResponse = sut.GetAllVacancyIds().ToList();

            var baseSearchDescriptor = new SearchDescriptor<TraineeshipSearchResult>();
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

            mockClient.Verify(c => c.Scroll<TraineeshipSearchResult>(It.IsAny<ScrollRequest>()), Times.Exactly(3));
        }

        private void AssertSearch(TraineeshipSearchRequestParameters parameters, string expectedJsonQuery)
        {
            var searchResponse = new Mock<ISearchResponse<TraineeshipSearchResult>>();
            searchResponse.Setup(s => s.Total).Returns(0);
            searchResponse.Setup(s => s.Documents).Returns(Enumerable.Empty<TraineeshipSearchResult>().ToList());

            Func<SearchDescriptor<TraineeshipSearchResult>, ISearchRequest> actualSearchDescriptorFunc = null;

            var mockClient = new Mock<IElasticClient>();

            mockClient.Setup(c => c.Search<TraineeshipSearchResult>(It.IsAny<Func<SearchDescriptor<TraineeshipSearchResult>, ISearchRequest>>()))
                .Callback<Func<SearchDescriptor<TraineeshipSearchResult>, ISearchRequest>>(d => actualSearchDescriptorFunc = d)
                .Returns(searchResponse.Object);

            var mockFactory = new Mock<IElasticClientFactory>();
            mockFactory.Setup(f => f.CreateClient(It.IsAny<Action<IApiCallDetails>>())).Returns(mockClient.Object);

            var mockLogger = new Mock<ILog>();

            var sut = new TraineeshipSearchClient(mockFactory.Object, "traineeships", mockLogger.Object);

            var response = sut.Search(parameters);

            var baseSearchDescriptor = new SearchDescriptor<TraineeshipSearchResult>();
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
