using System;
using Elasticsearch.Net;

namespace SFA.DAS.Elastic
{
    public class ElasticClientConfiguration
    {
        public Uri HostUri { get; private set; }
        public string CloudId { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public bool IsCloudConnectionConfigured => string.IsNullOrWhiteSpace(CloudId) == false;
        internal Action<IApiCallDetails> OnRequestCompleted { get; set; }
        internal bool EnableDebugMode { get; set; }

        public ElasticClientConfiguration(Uri hostUri)
        {
            if (hostUri == null)
            {
                throw new ArgumentNullException(nameof(hostUri));
            }

            HostUri = hostUri;
            CloudId = null;
        }

        public ElasticClientConfiguration(Uri hostUri, string username, string password)
        {
            if (hostUri == null)
            {
                throw new ArgumentNullException(nameof(hostUri));
            }

            SetCredentials(username, password);

            HostUri = hostUri;
            CloudId = null;
        }

        public ElasticClientConfiguration(string cloudId, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(cloudId))
            {
                throw new ArgumentNullException(nameof(cloudId));
            }

            SetCredentials(username, password);

            HostUri = null;
            CloudId = cloudId;
        }

        public IElasticClientFactory CreateClientFactory()
        {
            return new ElasticClientFactory(this);
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
    }
}