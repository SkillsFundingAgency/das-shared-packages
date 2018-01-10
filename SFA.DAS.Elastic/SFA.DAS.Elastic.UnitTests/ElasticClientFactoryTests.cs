using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Elastic.UnitTests
{
    public static class ElasticClientFactoryTests
    {
        public class When_getting_client : Test
        {
            private IElasticClient _client;
            private IElasticClientFactory _factory;

            private readonly IElasticConfiguration _configuration = new ElasticConfigurationStub
            {
                ElasticUrl = "http://localhost:9200"
            };

            private readonly IEnumerable<Mock<IIndexMapper>> _mappers = new List<Mock<IIndexMapper>>
            {
                new Mock<IIndexMapper>(),
                new Mock<IIndexMapper>(),
                new Mock<IIndexMapper>()
            };

            protected override void Given()
            {
                _factory = new ElasticClientFactory(_configuration, EnvironmentConfig, _mappers.Select(m => m.Object), Mock.Of<ILog>());
            }

            protected override void When()
            {
                _client = _factory.GetClient();
            }

            [Test]
            public void Then_should_return_client_with_correct_settings()
            {
                Assert.That(_client, Is.Not.Null);
                Assert.That(_client.ConnectionSettings.ThrowExceptions, Is.True);
            }

            [Test]
            public void Then_should_ensure_indices_exist()
            {
                foreach (var mapper in _mappers)
                {
                    mapper.Verify(m => m.EnureIndexExists(EnvironmentConfig, _client), Times.Once);
                }
            }
        }

        public class When_getting_client_multiple_times : Test
        {
            private IElasticClientFactory _factory;
            private IElasticClient _client1;
            private IElasticClient _client2;

            private readonly IElasticConfiguration _elasticConfig = new ElasticConfigurationStub
            {
                ElasticUrl = "http://localhost:9200"
            };

            private readonly IEnumerable<Mock<IIndexMapper>> _mappers = new List<Mock<IIndexMapper>>
            {
                new Mock<IIndexMapper>(),
                new Mock<IIndexMapper>(),
                new Mock<IIndexMapper>()
            };

            protected override void Given()
            {
                _factory = new ElasticClientFactory(_elasticConfig, EnvironmentConfig, _mappers.Select(m => m.Object), Mock.Of<ILog>());
            }

            protected override void When()
            {
                _client1 = _factory.GetClient();
                _client2 = _factory.GetClient();
            }

            [Test]
            public void Then_should_return_same_client()
            {
                Assert.That(_client1, Is.SameAs(_client2));
            }

            [Test]
            public void Then_should_ensure_indices_exist_once()
            {
                foreach (var mapper in _mappers)
                {
                    mapper.Verify(m => m.EnureIndexExists(EnvironmentConfig, _client1), Times.Once);
                }
            }
        }

        public class When_getting_client_with_authenticated_connection : Test
        {
            private IElasticClientFactory _factory;
            private IElasticClient _client;

            private readonly IElasticConfiguration _elasticConfiguration = new ElasticConfigurationStub
            {
                ElasticUrl = "http://localhost:9200",
                ElasticUsername = "elastic",
                ElasticPassword = "changeme"
            };

            protected override void Given()
            {
                _factory = new ElasticClientFactory(_elasticConfiguration, EnvironmentConfig, new List<IIndexMapper>(), Mock.Of<ILog>());
            }

            protected override void When()
            {
                _client = _factory.GetClient();
            }

            [Test]
            public void Then_should_return_client_with_basic_auth_enabled()
            {
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials.Username, Is.EqualTo(_elasticConfiguration.ElasticUsername));
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials.Password, Is.EqualTo(_elasticConfiguration.ElasticPassword));
            }
        }

        public class When_getting_client_with_unauthenticated_connection : Test
        {
            private IElasticClientFactory _factory;
            private IElasticClient _client;

            private readonly IElasticConfiguration _elasticConfiguration = new ElasticConfigurationStub
            {
                ElasticUrl = "http://localhost:9200",
                ElasticUsername = "",
                ElasticPassword = ""
            };

            protected override void Given()
            {
                _factory = new ElasticClientFactory(_elasticConfiguration, EnvironmentConfig, new List<IIndexMapper>(), Mock.Of<ILog>());
            }

            protected override void When()
            {
                _client = _factory.GetClient();
            }

            [Test]
            public void Then_should_return_client_with_basic_auth_disabled()
            {
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials, Is.Null);
            }
        }

        public class When_a_request_is_completed : Test
        {
            private IElasticClient _client;
            private IElasticClientFactory _factory;

            private readonly IElasticConfiguration _configuration = new ElasticConfigurationStub
            {
                ElasticUrl = "http://localhost:9200"
            };

            private readonly Mock<ILog> _log = new Mock<ILog>();
            private readonly Mock<IApiCallDetails> _apiCallDetails = new Mock<IApiCallDetails>();
            private string _debugInfo = "Foobar";

            protected override void Given()
            {
                _factory = new ElasticClientFactory(_configuration, EnvironmentConfig, new List<IIndexMapper>(), _log.Object);
                _client = _factory.GetClient();
                _apiCallDetails.Setup(r => r.DebugInformation).Returns(_debugInfo);
            }

            protected override void When()
            {
                _client.ConnectionSettings.OnRequestCompleted(_apiCallDetails.Object);
            }

            [Test]
            public void Then_should_log_debug_info()
            {
                _log.Verify(l => l.Debug(_debugInfo));
            }
        }

        private static readonly IEnvironmentConfiguration EnvironmentConfig = new EnvironmentConfiguration
        {
            EnvironmentName = "LOCAL"
        };

        private class ElasticConfigurationStub : IElasticConfiguration
        {
            public string ElasticUrl { get; set; }
            public string ElasticUsername { get; set; }
            public string ElasticPassword { get; set; }
        }
    }
}