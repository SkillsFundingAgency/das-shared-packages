using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace SFA.DAS.CosmosDb
{
    public interface IDocumentRepository<TDocument> where TDocument : class
    {
        Task Add(TDocument document, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
        IQueryable<TDocument> CreateQuery(FeedOptions feedOptions = null);
        Task<TDocument> GetById(Guid id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
        Task Remove(Guid id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
        Task Update(TDocument document, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    }
}