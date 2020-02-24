using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using Moq;
using Nest;
using NUnit.Framework;

namespace SFA.DAS.Elastic.UnitTests
{
    public static class ElasticClientConfigurationTests
    {
        public class When_configuring_elastic_with_no_settings : Test
        {
            private ElasticClientConfiguration _configuration;
            private Exception _ex;

            protected override void Given()
            {
            }

            protected override void When()
            {
                try
                {
                    _configuration = new ElasticClientConfiguration();
                    _configuration.CreateClientFactory();
                }
                catch (Exception ex)
                {
                    _ex = ex;
                }
            }

            [Test]
            public void Then_should_throw_exception()
            {
                Assert.That(_ex, Is.Not.Null);
                Assert.That(_ex.Message, Does.StartWith($"Connection pool should be configured using"));
            }
        }

        public class When_configuring_elastic_with_min_settings_using_single_node_connectionpool : Test
        {
            private ElasticClientFactory _factory;

            protected override void Given()
            {
            }

            protected override void When()
            {
                _factory = new ElasticClientConfiguration()
                    .UseSingleNodeConnectionPool(ElasticUrl, ElasticUsername, ElasticPassword)
                    .CreateClientFactory() as ElasticClientFactory;
            }

            [Test]
            public void Then_should_create_client_with_min_settings()
            {
                Assert.That(_factory, Is.Not.Null);
                Assert.That(_factory._configuration.IndexMappers, Is.Empty);
                Assert.That(_factory._configuration.HostUri, Is.EqualTo(new Uri(ElasticUrl)));
                Assert.That(_factory._configuration.CloudId, Is.Null);
                Assert.That(_factory._configuration.Username, Is.EqualTo(ElasticUsername));
                Assert.That(_factory._configuration.Password, Is.EqualTo(ElasticPassword));
            }
        }

        public class When_configuring_elastic_with_min_settings_using_cloud_connectionpool : Test
        {
            private ElasticClientFactory _factory;

            protected override void Given()
            {
            }

            protected override void When()
            {
                _factory = new ElasticClientConfiguration()
                    .UseCloudConnectionPool(CloudId, ElasticUsername, ElasticPassword)
                    .CreateClientFactory() as ElasticClientFactory;
            }

            [Test]
            public void Then_should_create_client_with_min_settings()
            {
                Assert.That(_factory, Is.Not.Null);
                Assert.That(_factory._configuration.IndexMappers, Is.Empty);
                Assert.That(_factory._configuration.HostUri, Is.Null);
                Assert.That(_factory._configuration.CloudId, Is.EqualTo(CloudId));
                Assert.That(_factory._configuration.Username, Is.EqualTo(ElasticUsername));
                Assert.That(_factory._configuration.Password, Is.EqualTo(ElasticPassword));
            }
        }

        public class When_configuring_elastic_with_max_settings : Test
        {
            private ElasticClientFactory _factory;
            private readonly Mock<Action<IApiCallDetails>> _onRequestCompleted = new Mock<Action<IApiCallDetails>>();
            private readonly Mock<IApiCallDetails> _apiCallDetails = new Mock<IApiCallDetails>();

            protected override void Given()
            {
            }

            protected override void When()
            {
                var conf = new ElasticClientConfiguration()
                    .SuffixEnvironmentNameToIndex(OverriddenEnvironmentName)
                    .UseSingleNodeConnectionPool(ElasticUrl, ElasticUsername, ElasticPassword)
                    .ScanForIndexMappers(typeof(ElasticClientConfigurationTests).Assembly)
                    .OnRequestCompletedCallbackAction(_onRequestCompleted.Object)
                    .UseDebugMode();
                _factory = conf.CreateClientFactory() as ElasticClientFactory;
            }

            [Test]
            public void Then_should_create_client_with_max_settings()
            {
                Assert.That(_factory, Is.Not.Null);
                Assert.That(_factory._configuration.EnvironmentName, Is.EqualTo(OverriddenEnvironmentName));
                Assert.That(_factory._configuration.IndexMappers, Is.Not.Null);
                Assert.That(_factory._configuration.IndexMappers.Count, Is.EqualTo(1));
                Assert.That(_factory._configuration.IndexMappers.Single(), Is.TypeOf<IndexMapperStub>());
                Assert.That(_factory._configuration.Username, Is.EqualTo(ElasticUsername));
                Assert.That(_factory._configuration.Password.Length, Is.EqualTo(ElasticPassword.Length));
                Assert.That(_factory._configuration.OnRequestCompleted, Is.Not.Null);
                Assert.That(_factory._configuration.EnableDebugMode, Is.True);
            }
        }

        public class When_configuring_with_index_mappers : Test
        {
            private ElasticClientFactory _factory;
            private readonly IEnumerable<Mock<IIndexMapper>> _mappers = new List<Mock<IIndexMapper>>
            {
                new Mock<IIndexMapper>(),
                new Mock<IIndexMapper>(),
                new Mock<IIndexMapper>()
            };
            protected override void Given()
            {
                
            }
            protected override void When()
            {
                _factory = new ElasticClientConfiguration()
                    .ScanForIndexMappers(typeof(ElasticClientConfigurationTests).Assembly)
                    .UseSingleNodeConnectionPool(ElasticUrl, ElasticUsername, ElasticPassword)
                    .SuffixEnvironmentNameToIndex(EnvironmentName)
                    .CreateClientFactory() as ElasticClientFactory;
                _factory._configuration.IndexMappers = _mappers.Select(m => m.Object).ToList();
                var client = _factory.CreateClient();
            }

            [Test]
            public void Then_should_ensure_indices_exist()
            {
                foreach (var mapper in _mappers)
                {
                    mapper.Verify(m => m.EnureIndexExistsAsync(EnvironmentName, It.IsAny<ElasticClient>()), Times.Once);
                }
            }
        }

        private const string OverriddenEnvironmentName = "at";
        private const string ElasticUrl = "http://localhost:9200";
        private const string ElasticUsername = "elastic";
        private const string ElasticPassword = "changeme";
        private const string CloudId = "cloudid:base64data";
        private const string EnvironmentName = "local";

        public class IndexMapperStub : IndexMapper<Stub>
        {
            protected override string IndexName => "stubs";
            
            protected override void Map(TypeMappingDescriptor<Stub> mapper)
            {
            }
        }

        public class Stub
        {
        }
    }
}