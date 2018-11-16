using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace SFA.DAS.CosmosDb
{
    public abstract class DocumentRepository<TDocument> : IDocumentRepository<TDocument> where TDocument : class, IDocument
    {
        private readonly IDocumentClient _documentClient;
        private readonly string _databaseName;
        private readonly string _collectionName;

        protected DocumentRepository(IDocumentClient documentClient, string databaseName, string collectionName)
        {
            _documentClient = documentClient;
            _databaseName = databaseName;
            _collectionName = collectionName;
        }

        public virtual Task Add(TDocument document, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (document.Id == Guid.Empty) throw new Exception("Id must not be Empty");
            return _documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), document, requestOptions, true, cancellationToken);
        }

        public virtual IQueryable<TDocument> CreateQuery(FeedOptions feedOptions = null)
        {
            return _documentClient.CreateDocumentQuery<TDocument>(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), feedOptions);
        }

        public virtual async Task<TDocument> GetById(Guid id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
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
                    return null;
                }

                throw;
            }
        }

        public virtual Task Remove(Guid id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            return _documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, id.ToString()), requestOptions, cancellationToken);
        }

        public virtual Task Update(TDocument document, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (document.Id == Guid.Empty) throw new Exception("Id must not be Empty");

            requestOptions = AddOptimisticLockingIfETagSetAndNoAccessConditionDefined(document, requestOptions);

            return _documentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, document.Id.ToString()), document, requestOptions, cancellationToken);
        }

        private RequestOptions AddOptimisticLockingIfETagSetAndNoAccessConditionDefined(IDocument document, RequestOptions requestOptions)
        {
            var options = requestOptions ?? new RequestOptions();
            if (options.AccessCondition == null)
            {
                if (!string.IsNullOrWhiteSpace(document.ETag))
                {
                    options.AccessCondition = new AccessCondition {
                        Condition = document.ETag,
                        Type = AccessConditionType.IfMatch
                    };
                }
            }
            return options;
        }
    }
}