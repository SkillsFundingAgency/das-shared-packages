using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Elastic.Extensions;

namespace SFA.DAS.Elastic.UnitTests
{
    public static class ElasticClientConfigurationTests
    {
        public class When_configuring_elastic_with_min_settings_using_single_node_connectionpool : Test
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

        public class When_configuring_elastic_with_min_settings_using_cloud_connectionpool : Test
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

        public class When_configuring_elastic_with_max_settings : Test
        {
            private ElasticClientFactory _factory;
            private readonly Mock<Action<IApiCallDetails>> _onRequestCompleted = new Mock<Action<IApiCallDetails>>();

            protected override void Given()
            {
            }

            protected override void When()
            {
                var conf = new ElasticClientConfiguration(new Uri(ElasticUrl), ElasticUsername, ElasticPassword);
                conf.OnRequestCompleted = _onRequestCompleted.Object;
                conf.EnableDebugMode = true;

                _factory = conf.CreateClientFactory() as ElasticClientFactory;
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

        private const string ElasticUrl = "http://localhost:9200";
        private const string ElasticUsername = "elastic";
        private const string ElasticPassword = "changeme";
        private const string CloudId = "cloudid:base64data";
    }
}