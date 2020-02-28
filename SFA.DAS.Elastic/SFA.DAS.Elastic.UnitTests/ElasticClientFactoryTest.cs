using Elasticsearch.Net;
using Moq;
using System;
using NUnit.Framework;

namespace SFA.DAS.Elastic.UnitTests
{
    public static class ElasticClientFactoryTest 
    {
        private const string ElasticUrl = "http://localhost:9200";
        private const string ElasticUsername = "elastic";
        private const string ElasticPassword = "changeme";

        public class When_creating_client_with_callback : Test
        {
            private ElasticClientFactory _factory;
            private readonly Mock<Action<IApiCallDetails>> _onRequestCompleted = new Mock<Action<IApiCallDetails>>();

            protected override void Given()
            {
            }

            protected override void When()
            {
                var conf = new ElasticClientConfiguration(new Uri(ElasticUrl), ElasticUsername, ElasticPassword);
                _factory = conf.CreateClientFactory() as ElasticClientFactory;
                _factory.CreateClient(_onRequestCompleted.Object);
            }

            [Test]
            public void Then_should_create_client_with_max_settings()
            {
                Assert.That(_factory, Is.Not.Null);
                Assert.That(_factory._configuration.Username, Is.EqualTo(ElasticUsername));
                Assert.That(_factory._configuration.Password.Length, Is.EqualTo(ElasticPassword.Length));
                Assert.That(_factory._configuration.OnRequestCompleted, Is.Not.Null);
                Assert.That(_factory._configuration.EnableDebugMode, Is.True);
            }
        }

        public class When_creating_client_without_callback : Test
        {
            private ElasticClientFactory _factory;

            protected override void Given()
            {
            }

            protected override void When()
            {
                var conf = new ElasticClientConfiguration(new Uri(ElasticUrl), ElasticUsername, ElasticPassword);
                _factory = conf.CreateClientFactory() as ElasticClientFactory;
                _factory.CreateClient();
            }

            [Test]
            public void Then_should_create_client_with_max_settings()
            {
                Assert.That(_factory, Is.Not.Null);
                Assert.That(_factory._configuration.Username, Is.EqualTo(ElasticUsername));
                Assert.That(_factory._configuration.Password.Length, Is.EqualTo(ElasticPassword.Length));
                Assert.That(_factory._configuration.OnRequestCompleted, Is.Null);
                Assert.That(_factory._configuration.EnableDebugMode, Is.False);
            }
        }
    }
}
