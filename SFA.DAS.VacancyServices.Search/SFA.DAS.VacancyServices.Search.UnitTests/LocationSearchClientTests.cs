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

    public class LocationClientTests
    {
        [Test]
        public void Search_ShouldReturnOrderedResults()
        {
            LocationSearchClient sut = new TestLocationSearchClient();

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
            searchReponse.Setup(s => s.Documents).Returns(Enumerable.Empty<LocationSearchResult>());

            Func<SearchDescriptor<LocationSearchResult>, SearchDescriptor<LocationSearchResult>> actualSearchDescriptorFunc = null;

            var mockClient = new Mock<IElasticClient>();

            mockClient.Setup(c => c.Search<LocationSearchResult>(It.IsAny<Func<SearchDescriptor<LocationSearchResult>, SearchDescriptor<LocationSearchResult>>>()))
                .Callback<Func<SearchDescriptor<LocationSearchResult>, SearchDescriptor<LocationSearchResult>>>(d => actualSearchDescriptorFunc = d)
                .Returns(searchReponse.Object);

            var factory = new Mock<IElasticSearchFactory>();
            factory.Setup(f => f.GetElasticClient(It.IsAny<string>())).Returns(mockClient.Object);

            var sut = new LocationSearchClient(factory.Object, new VacancyServicesSearchConfiguration());

            var response = searchFunc(sut);

            var baseSearchDescriptor = new SearchDescriptor<LocationSearchResult>();
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
