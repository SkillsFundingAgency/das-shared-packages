using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace SFA.DAS.CosmosDb
{
    public abstract class DocumentRepository<TDocument> : DocumentRepositoryRead<TDocument>, IDocumentRepository<TDocument> where TDocument : class, IDocument
    {
        protected DocumentRepository(IDocumentClient documentClient, string databaseName, string collectionName)
        : base(documentClient, databaseName, collectionName)
        {
        }

        public virtual Task Add(TDocument document, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (document.Id == Guid.Empty) throw new Exception("Id must not be Empty");
            return DocumentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), document, requestOptions, true, cancellationToken);
        }

        public virtual Task Remove(Guid id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            return DocumentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id.ToString()), requestOptions, cancellationToken);
        }

        public virtual Task Update(TDocument document, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (document.Id == Guid.Empty) throw new Exception("Id must not be Empty");

            requestOptions = AddOptimisticLockingIfETagSetAndNoAccessConditionDefined(document, requestOptions);

            return DocumentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, document.Id.ToString()), document, requestOptions, cancellationToken);
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