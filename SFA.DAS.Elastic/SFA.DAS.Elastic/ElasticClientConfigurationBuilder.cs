using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elasticsearch.Net;

namespace SFA.DAS.Elastic
{
    public class ElasticClientConfiguration : IDisposable
    {
        internal Action<IApiCallDetails> OnRequestCompleted { get; private set; }
        internal string Username { get; private set; }
        internal string Password { get; private set; }
        internal string EnvironmentName { get; private set; }
        internal Uri HostUri { get; private set; }
        internal string CloudId { get; private set; }
        internal bool EnableDebugMode { get; private set; }
        internal List<IIndexMapper> IndexMappers { get; set; } = new List<IIndexMapper>();
        internal bool IsCloudConnectionConfigured => string.IsNullOrWhiteSpace(CloudId) == false;

        public ElasticClientConfiguration OnRequestCompletedCallbackAction(Action<IApiCallDetails> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            OnRequestCompleted = handler;

            return this;
        }

        public ElasticClientConfiguration SuffixEnvironmentNameToIndex(string environmentName)
        {
            if (string.IsNullOrWhiteSpace(environmentName))
            {
                throw new ArgumentNullException(nameof(environmentName));
            }

            EnvironmentName = environmentName.ToLower();

            return this;
        }

        public ElasticClientConfiguration ScanForIndexMappers(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (assembly != null)
            {
                IndexMappers.AddRange(assembly
                    .GetExportedTypes()
                    .Where(t =>
                        !t.IsAbstract &&
                        !t.IsInterface &&
                        typeof(IIndexMapper).IsAssignableFrom(t)
                    )
                    .Select(Activator.CreateInstance)
                    .Cast<IIndexMapper>());
            }

            return this;
        }

        private void SetCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            Username = username;
            Password = password;
        }

        public ElasticClientConfiguration UseSingleNodeConnectionPool(string hostUrl, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(hostUrl))
            {
                throw new ArgumentNullException(nameof(hostUrl));
            }

            if(Uri.TryCreate(hostUrl, UriKind.Absolute, out Uri uri) == false)
            {
                throw new ArgumentException($"{hostUrl} is not a valid url", nameof(hostUrl));
            }

            SetCredentials(username, password);

            HostUri = uri;
            CloudId = null;

            return this;
        }

        public ElasticClientConfiguration UseCloudConnectionPool(string cloudId, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(cloudId))
            {
                throw new ArgumentNullException(nameof(cloudId));
            }

            SetCredentials(username, password);

            HostUri = null;
            CloudId = cloudId;

            return this;
        }

        public ElasticClientConfiguration UseDebugMode()
        {
            EnableDebugMode = true;

            return this;
        }

        public IElasticClientFactory CreateClientFactory()
        {
            ValidateConfiguration();

            return new ElasticClientFactory(this);
        }

        private void ValidateConfiguration()
        {
            if (HostUri == null && string.IsNullOrWhiteSpace(CloudId))
            {
                throw new Exception($"Connection pool should be configured using '{nameof(UseSingleNodeConnectionPool)}' or '{nameof(UseCloudConnectionPool)}' before calling '{nameof(CreateClientFactory)}'.");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _isDisposed;
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                foreach (var indexMapper in IndexMappers)
                {
                    indexMapper.Dispose();
                }
            }

            _isDisposed = true;
        }
    }
}