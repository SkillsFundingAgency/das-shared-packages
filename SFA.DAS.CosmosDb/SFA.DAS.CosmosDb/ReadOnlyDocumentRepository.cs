using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace SFA.DAS.CosmosDb
{
    public abstract class ReadOnlyDocumentRepository<TDocument> : IReadOnlyDocumentRepository<TDocument> where TDocument : class
    {
        private readonly DocumentRepositoryRead<TDocument> _documentRepositoryRead;

        protected ReadOnlyDocumentRepository(IDocumentClient documentClient, string databaseName, string collectionName)
        {
            _documentRepositoryRead = new DocumentRepositoryRead<TDocument>(documentClient, databaseName, collectionName);
        }

        public virtual IQueryable<TDocument> CreateQuery(FeedOptions feedOptions = null)
        {
            return _documentRepositoryRead.CreateQuery(feedOptions);
        }

        public virtual Task<TDocument> GetById(Guid id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            return _documentRepositoryRead.GetById(id, requestOptions, cancellationToken);
        }
    }
}