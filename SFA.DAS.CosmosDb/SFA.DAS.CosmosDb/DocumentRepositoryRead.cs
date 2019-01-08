using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace SFA.DAS.CosmosDb
{
    internal class DocumentRepositoryRead<TDocument> where TDocument : class
    {
        private readonly IDocumentClient _documentClient;
        private readonly string _databaseName;
        private readonly string _collectionName;

        internal DocumentRepositoryRead(IDocumentClient documentClient, string databaseName, string collectionName)
        {
            _documentClient = documentClient;
            _databaseName = databaseName;
            _collectionName = collectionName;
        }

        internal IQueryable<TDocument> CreateQuery(FeedOptions feedOptions = null)
        {
            return _documentClient.CreateDocumentQuery<TDocument>(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), feedOptions);
        }

        internal async Task<TDocument> GetById(Guid id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _documentClient.ReadDocumentAsync<TDocument>(UriFactory.CreateDocumentUri(_databaseName, _collectionName, id.ToString()), requestOptions, cancellationToken).ConfigureAwait(false);

                return response.Document;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return default;
                }

                throw;
            }
        }
    }
}