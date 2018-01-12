using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using Moq;
using Nest;
using NUnit.Framework;

namespace SFA.DAS.Elastic.UnitTests
{
    public static class ElasticClientFactoryTests
    {
        public class When_getting_client : Test
        {
            private IElasticClient _client;
            private IElasticClientFactory _factory;

            protected override void Given()
            {
                _factory = new ElasticClientFactory(EnvironmentName, ElasticUrl, null, null, null, null);
            }

            protected override void When()
            {
                _client = _factory.CreateClient();
            }

            [Test]
            public void Then_should_return_client_with_correct_settings()
            {
                Assert.That(_client, Is.Not.Null);
                Assert.That(_client.ConnectionSettings.ConnectionPool, Is.TypeOf<SingleNodeConnectionPool>());
                Assert.That(_client.ConnectionSettings.ThrowExceptions, Is.True);
            }
        }

        public class When_getting_client_multiple_times : Test
        {
            private IElasticClientFactory _factory;
            private IElasticClient _client1;
            private IElasticClient _client2;

            protected override void Given()
            {
                _factory = new ElasticClientFactory(EnvironmentName, ElasticUrl, null, null, null, null);
            }

            protected override void When()
            {
                _client1 = _factory.CreateClient();
                _client2 = _factory.CreateClient();
            }

            [Test]
            public void Then_should_create_multiple_clients()
            {
                Assert.That(_client1, Is.Not.Null);
                Assert.That(_client2, Is.Not.Null);
                Assert.That(_client1, Is.Not.SameAs(_client2));
            }

            [Test]
            public void Then_should_create_single_connection()
            {
                Assert.That(_client1.ConnectionSettings, Is.SameAs(_client2.ConnectionSettings));
            }
        }

        public class When_getting_client_with_authenticated_connection : Test
        {
            private IElasticClientFactory _factory;
            private IElasticClient _client;

            protected override void Given()
            {
                _factory = new ElasticClientFactory(EnvironmentName, ElasticUrl, ElasticUsername, ElasticPassword, null, null);
            }

            protected override void When()
            {
                _client = _factory.CreateClient();
            }

            [Test]
            public void Then_should_return_client_with_basic_auth_enabled()
            {
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials, Is.Not.Null);
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials.Username, Is.EqualTo(ElasticUsername));
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials.Password, Is.EqualTo(ElasticPassword));
            }
        }

        public class When_getting_client_with_unauthenticated_connection : Test
        {
            private IElasticClientFactory _factory;
            private IElasticClient _client;

            protected override void Given()
            {
                _factory = new ElasticClientFactory(EnvironmentName, ElasticUrl, null, null, null, null);
            }

            protected override void When()
            {
                _client = _factory.CreateClient();
            }

            [Test]
            public void Then_should_return_client_with_basic_auth_disabled()
            {
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials, Is.Null);
            }
        }

        public class When_getting_client_and_on_request_completed_is_configured : Test
        {
            private IElasticClientFactory _factory;
            private IElasticClient _client;
            private readonly Mock<IApiCallDetails> _apiCallDetails = new Mock<IApiCallDetails>();
            private readonly Mock<Action<IApiCallDetails>> _onRequestCompleted = new Mock<Action<IApiCallDetails>>();

            protected override void Given()
            {
                _factory = new ElasticClientFactory(EnvironmentName, ElasticUrl, null, null, _onRequestCompleted.Object, null);
                _client = _factory.CreateClient();
            }

            protected override void When()
            {
                _client.ConnectionSettings.OnRequestCompleted(_apiCallDetails.Object);
            }

            [Test]
            public void Then_should_invoke_action()
            {
                _onRequestCompleted.Verify(a => a(_apiCallDetails.Object));
            }
        }

        public class When_getting_client_and_index_mappers_are_configured : Test
        {
            private IElasticClient _client;
            private IElasticClientFactory _factory;

            private readonly IEnumerable<Mock<IIndexMapper>> _mappers = new List<Mock<IIndexMapper>>
            {
                new Mock<IIndexMapper>(),
                new Mock<IIndexMapper>(),
                new Mock<IIndexMapper>()
            };

            protected override void Given()
            {
                _factory = new ElasticClientFactory(EnvironmentName, ElasticUrl, "", "", null, () => _mappers.Select(m => m.Object));
            }

            protected override void When()
            {
                _client = _factory.CreateClient();
            }

            [Test]
            public void Then_should_ensure_indices_exist()
            {
                foreach (var mapper in _mappers)
                {
                    mapper.Verify(m => m.EnureIndexExistsAsync(EnvironmentName, _client), Times.Once);
                }
            }
        }

        private const string EnvironmentName = "LOCAL";
        private const string ElasticUrl = "http://localhost:9200";
        private const string ElasticUsername = "elastic";
        private const string ElasticPassword = "changeme";
    }
}