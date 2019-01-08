using Microsoft.Azure.Documents;

namespace SFA.DAS.CosmosDb
{
    public abstract class ReadOnlyDocumentRepository<TDocument> : DocumentRepositoryRead<TDocument>, IReadOnlyDocumentRepository<TDocument> where TDocument : class
    {
        protected ReadOnlyDocumentRepository(IDocumentClient documentClient, string databaseName, string collectionName)
        : base(documentClient, databaseName, collectionName)
        {
        }
    }
}