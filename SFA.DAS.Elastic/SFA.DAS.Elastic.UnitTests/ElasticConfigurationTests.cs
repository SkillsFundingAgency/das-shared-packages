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
            private IElasticClientFactory _factory;

            protected override void Given()
            {
            }

            protected override void When()
            {
                _factory = new ElasticConfiguration()
                    .UseSingleNodeConnectionPool(ElasticUrl)
                    .CreateClientFactory();
            }

            [Test]
            public void Then_should_create_client_with_min_settings()
            {
                Assert.That(_factory, Is.Not.Null);
                Assert.That(_factory.EnvironmentName, Is.EqualTo(EnvironmentName));
                Assert.That(_factory.IndexMappersFactory, Is.Null);
                Assert.That(_factory.ConnectionSettings.ConnectionPool, Is.TypeOf<SingleNodeConnectionPool>());
                Assert.That(_factory.ConnectionSettings.ConnectionPool.Nodes.Count, Is.EqualTo(1));
                Assert.That(_factory.ConnectionSettings.ConnectionPool.Nodes.Single().Uri, Is.EqualTo(new Uri(ElasticUrl)));
                Assert.That(_factory.ConnectionSettings.BasicAuthenticationCredentials, Is.Null);
            }
        }

        public class When_configuring_elastic_with_max_settings : Test
        {
            private IElasticClientFactory _factory;
            private readonly Mock<Action<IApiCallDetails>> _onRequestCompleted = new Mock<Action<IApiCallDetails>>();
            private readonly Mock<IApiCallDetails> _apiCallDetails = new Mock<IApiCallDetails>();

            protected override void Given()
            {
            }

            protected override void When()
            {
                _factory = new ElasticConfiguration()
                    .OverrideEnvironmentName(OverriddenEnvironmentName)
                    .UseSingleNodeConnectionPool(ElasticUrl)
                    .UseBasicAuthentication(ElasticUsername, ElasticPassword)
                    .ScanForIndexMappers(typeof(ElasticConfigurationTests).Assembly)
                    .OnRequestCompleted(_onRequestCompleted.Object)
                    .CreateClientFactory();
                
                _factory?.ConnectionSettings?.OnRequestCompleted(_apiCallDetails.Object);
            }

            [Test]
            public void Then_should_create_client_with_max_settings()
            {
                Assert.That(_factory, Is.Not.Null);
                Assert.That(_factory.EnvironmentName, Is.EqualTo(OverriddenEnvironmentName));
                Assert.That(_factory.IndexMappersFactory, Is.Not.Null);

                var indexMappers = _factory.IndexMappersFactory().ToList();
                
                Assert.That(indexMappers.Count, Is.EqualTo(1));
                Assert.That(indexMappers.Single(), Is.TypeOf<IndexMapperStub>());
                Assert.That(_factory.ConnectionSettings.ConnectionPool, Is.TypeOf<SingleNodeConnectionPool>());
                Assert.That(_factory.ConnectionSettings.ConnectionPool.Nodes.Count, Is.EqualTo(1));
                Assert.That(_factory.ConnectionSettings.ConnectionPool.Nodes.Single().Uri, Is.EqualTo(new Uri(ElasticUrl)));
                Assert.That(_factory.ConnectionSettings.BasicAuthenticationCredentials, Is.Not.Null);
                Assert.That(_factory.ConnectionSettings.BasicAuthenticationCredentials.Username, Is.EqualTo(ElasticUsername));
                Assert.That(_factory.ConnectionSettings.BasicAuthenticationCredentials.Password, Is.EqualTo(ElasticPassword));

                _onRequestCompleted.Verify(a => a(_apiCallDetails.Object));
            }
        }

        private static readonly string EnvironmentName = CloudConfigurationManager.GetSetting("EnvironmentName");
        private const string OverriddenEnvironmentName = "AT";
        private const string ElasticUrl = "http://localhost:9200";
        private const string ElasticUsername = "elastic";
        private const string ElasticPassword = "changeme";

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