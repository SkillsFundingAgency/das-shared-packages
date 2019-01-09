using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace SFA.DAS.CosmosDb
{
    public class DocumentRepository<TDocument> : IDocumentRepository<TDocument> where TDocument : class, IDocument
    {
        private readonly ReadOnlyDocumentRepository<TDocument> _readOnlyDocumentRepository;
        private readonly IDocumentClient _documentClient;
        private readonly string _databaseName;
        private readonly string _collectionName;
        
        public DocumentRepository(IDocumentClient documentClient, string databaseName, string collectionName)
        {
            _readOnlyDocumentRepository = new ReadOnlyDocumentRepository<TDocument>(documentClient, databaseName, collectionName);
            _documentClient = documentClient;
            _databaseName = databaseName;
            _collectionName = collectionName;
        }

        public virtual IQueryable<TDocument> CreateQuery(FeedOptions feedOptions = null)
        {
            return _readOnlyDocumentRepository.CreateQuery(feedOptions);
        }

        public virtual Task<TDocument> GetById(Guid id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            return _readOnlyDocumentRepository.GetById(id, requestOptions, cancellationToken);
        }

        public virtual Task Add(TDocument document, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (document.Id == Guid.Empty) throw new Exception("Id must not be Empty");
            return _documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), document, requestOptions, true, cancellationToken);
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