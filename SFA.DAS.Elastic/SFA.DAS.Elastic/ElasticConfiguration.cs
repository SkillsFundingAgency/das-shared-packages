using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elasticsearch.Net;
using Microsoft.Azure;

namespace SFA.DAS.Elastic
{
    public class ElasticConfiguration
    {
        private static readonly object Lock = new object();

        private Action<IApiCallDetails> _onRequestCompleted;
        private string _username;
        private string _password;
        private string _environmentName;
        private Assembly _indexMappersAssembly;
        private string _url;
        private List<IIndexMapper> _indexMappers;

        public ElasticConfiguration OnRequestCompleted(Action<IApiCallDetails> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _onRequestCompleted = handler;

            return this;
        }

        public ElasticConfiguration OverrideEnvironmentName(string environmentName)
        {
            if (string.IsNullOrWhiteSpace(environmentName))
            {
                throw new ArgumentNullException(nameof(environmentName));
            }

            _environmentName = environmentName;

            return this;
        }

        public ElasticConfiguration ScanForIndexMappers(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            _indexMappersAssembly = assembly;

            return this;
        }

        public ElasticConfiguration UseBasicAuthentication(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            _username = username;
            _password = password;

            return this;
        }

        public ElasticConfiguration UseSingleNodeConnectionPool(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            _url = url;

            return this;
        }

        public IElasticClientFactory CreateClientFactory()
        {
            if (string.IsNullOrWhiteSpace(_url))
            {
                throw new Exception($"Node should be configured using {nameof(UseSingleNodeConnectionPool)} before calling {nameof(CreateClientFactory)}.");
            }

            Func<IEnumerable<IIndexMapper>> indexMappersFactory = null;

            if (_indexMappersAssembly != null)
            {
                indexMappersFactory = CreateIndexMappers;
            }

            return new ElasticClientFactory(
                _environmentName ?? CloudConfigurationManager.GetSetting("EnvironmentName"),
                _url,
                _username,
                _password,
                _onRequestCompleted,
                indexMappersFactory
            );
        }

        private IEnumerable<IIndexMapper> CreateIndexMappers()
        {
            // Locking for now until StructureMap singleton bug is fixed https://github.com/structuremap/structuremap/pull/625
            lock (Lock)
            {
                return _indexMappers ?? (_indexMappers = _indexMappersAssembly
                    .GetExportedTypes()
                    .Where(t =>
                        !t.IsAbstract &&
                        !t.IsInterface &&
                        typeof(IIndexMapper).IsAssignableFrom(t)
                    )
                    .Select(Activator.CreateInstance)
                    .Cast<IIndexMapper>()
                    .ToList());
            }
        }
    }
}