﻿namespace SFA.DAS.VacancyServices.Search.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Entities;
    using FluentAssertions;
    using FluentAssertions.Json;
    using Moq;
    using Nest;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using SFA.DAS.Elastic;
    using SFA.DAS.NLog.Logger;

    public class LocationClientTests
    {
        [Test]
        public void Search_ShouldReturnOrderedResults()
        {
            var mockClient = new Mock<IElasticClient>();
            var mockFactory = new Mock<IElasticClientFactory>();
            mockFactory.Setup(f => f.CreateClient()).Returns(mockClient.Object);
            LocationSearchClient sut = new TestLocationSearchClient(mockFactory.Object);

            const string searchTerm = "coventry";

            var response = sut.Search(searchTerm);

            var locations = response.Locations.ToList();

            locations[0].Name.Should().Be("search-exact");
            locations[1].Name.Should().Be("search-prefixed");
            locations[2].Name.Should().Be("search-fuzzy");
        }

        [Test]
        public void SearchExact_ShouldSearchForExactValues()
        {
            const string searchTerm = "coventry";

            const string expectedJsonQuery = "{\"from\":0,\"size\":63,\"query\":{\"function_score\":{\"functions\":[{\"field_value_factor\":{\"field\":\"size\"}}],\"query\":{\"match\":{\"name\":{\"query\":\"coventry\"}}},\"score_mode\":\"sum\"}}}";

            AssertSearch(c => c.SearchExact(searchTerm, 63), expectedJsonQuery);
        }

        [Test]
        public void SearchPrefixed_ShouldSearchForPrefixedValues()
        {
            const string searchTerm = "coventry";

            const string expectedJsonQuery = "{\"from\":0,\"size\":63,\"query\":{\"function_score\":{\"functions\":[{\"field_value_factor\":{\"field\":\"size\"}}],\"query\":{\"prefix\":{\"name\":{\"value\":\"coventry\"}}},\"score_mode\":\"sum\"}}}";

            AssertSearch(c => c.SearchPrefixed(searchTerm, 63), expectedJsonQuery);
        }

        [Test]
        public void SearchFuzzy_ShouldSearchForFuzzyMatchesValues()
        {
            const string searchTerm = "coventry";

            const string expectedJsonQuery = "{\"from\":0,\"size\":63,\"query\":{\"function_score\":{\"functions\":[{\"field_value_factor\":{\"field\":\"size\"}}],\"query\":{\"bool\":{\"should\":[{\"fuzzy\":{\"name\":{\"prefix_length\":1,\"value\":\"coventry\",\"boost\":2.0}}},{\"fuzzy\":{\"county\":{\"prefix_length\":1,\"value\":\"coventry\",\"boost\":1.0}}}]}},\"score_mode\":\"sum\"}}}";

            AssertSearch(c => c.SearchFuzzy(searchTerm, 63), expectedJsonQuery);
        }

        private void AssertSearch(Func<LocationSearchClient, IEnumerable<LocationSearchResult>> searchFunc, string expectedJsonQuery)
        {
            var searchReponse = new Mock<ISearchResponse<LocationSearchResult>>();
            searchReponse.Setup(s => s.Total).Returns(0);
            searchReponse.Setup(s => s.Documents).Returns(Enumerable.Empty<LocationSearchResult>().ToList());
            searchReponse.Setup(s => s.Documents).Returns(new Mock<IReadOnlyCollection<LocationSearchResult>>().Object);
            
            Func<SearchDescriptor<LocationSearchResult>, ISearchRequest> actualSearchDescriptorFunc = null;

            var mockClient = new Mock<IElasticClient>();

            mockClient.Setup(c => c.Search<LocationSearchResult>(It.IsAny<Func<SearchDescriptor<LocationSearchResult>, ISearchRequest>>()))
                .Callback<Func<SearchDescriptor<LocationSearchResult>, ISearchRequest>>(d => actualSearchDescriptorFunc = d)
                .Returns(searchReponse.Object);

            var mockFactory = new Mock<IElasticClientFactory>();
            mockFactory.Setup(f => f.CreateClient()).Returns(mockClient.Object);

            var sut = new LocationSearchClient(mockFactory.Object, "locations");

            var response = searchFunc(sut);

            var baseSearchDescriptor = new SearchDescriptor<LocationSearchResult>();
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
