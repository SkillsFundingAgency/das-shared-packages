using System;
using System.Linq;
using Elasticsearch.Net;
using Microsoft.Azure;
using Moq;
using Nest;
using NUnit.Framework;

namespace SFA.DAS.Elastic.UnitTests
{
    public static class ElasticConfigurationTests
    {
        public class When_configuring_elastic_with_no_settings : Test
        {
            private IElasticClientFactory _factory;
            private Exception _ex;

            protected override void Given()
            {
            }

            protected override void When()
            {
                try
                {
                    _factory = new ElasticConfiguration().CreateClientFactory();
                }
                catch (Exception ex)
                {
                    _ex = ex;
                }
            }

            [Test]
            public void Then_should_throw_exception()
            {
                Assert.That(_factory, Is.Null);
                Assert.That(_ex, Is.Not.Null);
                Assert.That(_ex.Message, Does.StartWith($"Node should be configured using {nameof(ElasticConfiguration.UseSingleNodeConnectionPool)} before calling {nameof(ElasticConfiguration.CreateClientFactory)}."));
            }
        }

        public class When_configuring_elastic_with_min_settings : Test
        {
            private IElasticClient _client;

            protected override void Given()
            {
            }

            protected override void When()
            {
                _client = new ElasticConfiguration()
                    .UseSingleNodeConnectionPool(ElasticUrl)
                    .CreateClientFactory()
                    .CreateClient();
            }

            [Test]
            public void Then_should_create_client_with_min_settings()
            {
                Assert.That(_client, Is.Not.Null);
                Assert.That(_client.ConnectionSettings.ConnectionPool, Is.TypeOf<SingleNodeConnectionPool>());
                Assert.That(_client.ConnectionSettings.ConnectionPool.Nodes.Count, Is.EqualTo(1));
                Assert.That(_client.ConnectionSettings.ConnectionPool.Nodes.Single().Uri, Is.EqualTo(new Uri(ElasticUrl)));
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials, Is.Null);
                Assert.That(_client.ConnectionSettings.DefaultIndices.Count, Is.Zero);
            }
        }

        public class When_configuring_elastic_with_default_environment_name : Test
        {
            private IElasticClient _client;

            protected override void Given()
            {
            }

            protected override void When()
            {
                _client = new ElasticConfiguration()
                    .UseSingleNodeConnectionPool(ElasticUrl)
                    .ScanForIndexMappers(typeof(ElasticConfigurationTests).Assembly)
                    .CreateClientFactory()
                    .CreateClient();
            }

            [Test]
            public void Then_should_create_indices_with_default_environment_name_prefix()
            {
                Assert.That(_client, Is.Not.Null);
                Assert.That(_client.ConnectionSettings.DefaultIndices.Count, Is.EqualTo(1));
                Assert.That(_client.ConnectionSettings.DefaultIndices.TryGetValue(typeof(Stub), out var indexName), Is.True);
                Assert.That(indexName, Is.Not.Null);
                Assert.That(indexName, Is.EqualTo(DefaultEnvironmentStubsIndexName));
            }
        }

        public class When_configuring_elastic_with_max_settings : Test
        {
            private IElasticClient _client;
            private readonly Mock<Action<IApiCallDetails>> _onRequestCompleted = new Mock<Action<IApiCallDetails>>();
            private readonly Mock<IApiCallDetails> _apiCallDetails = new Mock<IApiCallDetails>();

            protected override void Given()
            {
            }

            protected override void When()
            {
                _client = new ElasticConfiguration()
                    .OverrideEnvironmentName(EnvironmentName)
                    .UseSingleNodeConnectionPool(ElasticUrl)
                    .UseBasicAuthentication(ElasticUsername, ElasticPassword)
                    .ScanForIndexMappers(typeof(ElasticConfigurationTests).Assembly)
                    .OnRequestCompleted(_onRequestCompleted.Object)
                    .CreateClientFactory()
                    .CreateClient();
                
                _client?.ConnectionSettings?.OnRequestCompleted(_apiCallDetails.Object);
            }

            [Test]
            public void Then_should_create_client_with_max_settings()
            {
                Assert.That(_client, Is.Not.Null);
                Assert.That(_client.ConnectionSettings.ConnectionPool, Is.TypeOf<SingleNodeConnectionPool>());
                Assert.That(_client.ConnectionSettings.ConnectionPool.Nodes.Count, Is.EqualTo(1));
                Assert.That(_client.ConnectionSettings.ConnectionPool.Nodes.Single().Uri, Is.EqualTo(new Uri(ElasticUrl)));
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials, Is.Not.Null);
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials.Username, Is.EqualTo(ElasticUsername));
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials.Password, Is.EqualTo(ElasticPassword));
                Assert.That(_client.ConnectionSettings.DefaultIndices.Count, Is.EqualTo(1));
                Assert.That(_client.ConnectionSettings.DefaultIndices.TryGetValue(typeof(Stub), out var indexName), Is.True);
                Assert.That(indexName, Is.Not.Null);
                Assert.That(indexName, Is.EqualTo(OveriddenEnvironmentStubsIndexName));

                _onRequestCompleted.Verify(a => a(_apiCallDetails.Object));
            }
        }

        private const string EnvironmentName = "AT";
        private const string ElasticUrl = "http://localhost:9200";
        private const string ElasticUsername = "elastic";
        private const string ElasticPassword = "changeme";
        private const string StubsIndexName = "stubs";
        private static readonly string DefaultEnvironmentStubsIndexName = $"{CloudConfigurationManager.GetSetting("EnvironmentName").ToLower()}-{StubsIndexName}";
        private static readonly string OveriddenEnvironmentStubsIndexName = $"{EnvironmentName.ToLower()}-{StubsIndexName}";

        public class IndexMapperStub : IndexMapper<Stub>
        {
            protected override string IndexName => StubsIndexName;
            
            protected override void Map(TypeMappingDescriptor<Stub> mapper)
            {
            }
        }

        public class Stub
        {
        }
    }
}