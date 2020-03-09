using System;
using NUnit.Framework;

namespace SFA.DAS.Elastic.UnitTests
{
    public static class ElasticClientConfigurationTests
    {
        public class When_using_single_node_connectionpool : Test
        {
            private ElasticClientFactory _factory;

            protected override void Given()
            {
            }

            protected override void When()
            {
                _factory = new ElasticClientConfiguration(new Uri(ElasticUrl), ElasticUsername, ElasticPassword)
                    .CreateClientFactory() as ElasticClientFactory;
            }

            [Test]
            public void Then_should_create_client_with_min_settings()
            {
                Assert.That(_factory, Is.Not.Null);
                Assert.That(_factory._configuration.HostUri, Is.EqualTo(new Uri(ElasticUrl)));
                Assert.That(_factory._configuration.CloudId, Is.Null);
                Assert.That(_factory._configuration.Username, Is.EqualTo(ElasticUsername));
                Assert.That(_factory._configuration.Password, Is.EqualTo(ElasticPassword));
            }
        }

        public class When_using_cloud_connectionpool : Test
        {
            private ElasticClientFactory _factory;

            protected override void Given()
            {
            }

            protected override void When()
            {
                _factory = new ElasticClientConfiguration(CloudId, ElasticUsername, ElasticPassword)
                    .CreateClientFactory() as ElasticClientFactory;
            }

            [Test]
            public void Then_should_create_client_with_min_settings()
            {
                Assert.That(_factory, Is.Not.Null);
                Assert.That(_factory._configuration.HostUri, Is.Null);
                Assert.That(_factory._configuration.CloudId, Is.EqualTo(CloudId));
                Assert.That(_factory._configuration.Username, Is.EqualTo(ElasticUsername));
                Assert.That(_factory._configuration.Password, Is.EqualTo(ElasticPassword));
            }
        }

        

        private const string ElasticUrl = "http://localhost:9200";
        private const string ElasticUsername = "elastic";
        private const string ElasticPassword = "changeme";
        private const string CloudId = "cloudid:base64data";
    }
}