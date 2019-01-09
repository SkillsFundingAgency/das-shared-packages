using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace SFA.DAS.CosmosDb
{
    public class ReadOnlyDocumentRepository<TDocument> : IReadOnlyDocumentRepository<TDocument> where TDocument : class
    {
        protected readonly IDocumentClient DocumentClient;
        protected readonly string DatabaseName;
        protected readonly string CollectionName;

        public ReadOnlyDocumentRepository(IDocumentClient documentClient, string databaseName, string collectionName)
        {
            DocumentClient = documentClient;
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }
        
        public virtual IQueryable<TDocument> CreateQuery(FeedOptions feedOptions = null)
        {
            return DocumentClient.CreateDocumentQuery<TDocument>(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), feedOptions);
        }

        public virtual async Task<TDocument> GetById(Guid id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await DocumentClient.ReadDocumentAsync<TDocument>(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id.ToString()), requestOptions, cancellationToken).ConfigureAwait(false);

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