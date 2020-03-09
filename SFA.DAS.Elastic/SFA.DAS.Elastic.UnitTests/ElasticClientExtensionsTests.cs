using NUnit.Framework;
using Nest;
using SFA.DAS.Elastic.Extensions;
using System.Collections.Generic;
using System.Linq;
using Moq;
using System;
using Elasticsearch.Net;

namespace SFA.DAS.Elastic.UnitTests
{
    public static class ElasticClientExtensionsTests
    {
        private const string ElasticUrl = "http://localhost:9200";
        private const string EnvironmentName = "local";
        private const string ElasticUsername = "elastic";
        private const string ElasticPassword = "changeme";

        public class When_configuring_with_index_mappers : Test
        {
            private readonly IEnumerable<Mock<IIndexMapper>> _mappers = new List<Mock<IIndexMapper>>
            {
                new Mock<IIndexMapper>(),
                new Mock<IIndexMapper>(),
                new Mock<IIndexMapper>()
            };

            protected override void Given()
            {}

            protected override void When()
            {
                new ElasticClientConfiguration(new Uri(ElasticUrl))
                    .CreateClientFactory()
                    .CreateClient()
                    .CreateIndicesIfNotExistsAsync(_mappers.Select(m => m.Object), EnvironmentName)
                    .Wait();
            }

            [Test]
            public void Then_should_ensure_indices_exist()
            {
                foreach (var mapper in _mappers)
                {
                    mapper.Verify(m => m.EnureIndexExistsAsync(It.IsAny<ElasticClient>(), EnvironmentName), Times.Once);
                }
            }
        }

        public class When_getting_index_mappers_from_assembly : Test
        {
            private List<IIndexMapper> _indexMappers;

            protected override void Given()
            {}

            protected override void When()
            {
                _indexMappers = typeof(ElasticClientConfigurationTests).Assembly.GetIndexMappers();
            }

            [Test]
            public void Then_should_return_all_mappers()
            {
                Assert.That(_indexMappers, Is.Not.Null);
                Assert.That(_indexMappers.Count, Is.EqualTo(1));
                Assert.That(_indexMappers.Single(), Is.TypeOf<IndexMapperStub>());
            }
        }

        public class IndexMapperStub : IndexMapper<Stub>
        {
            public IndexMapperStub() : base("stubs")
            {}

            protected override void Map(TypeMappingDescriptor<Stub> mapper)
            {}
        }

        public class Stub
        {
        }
    }
}
