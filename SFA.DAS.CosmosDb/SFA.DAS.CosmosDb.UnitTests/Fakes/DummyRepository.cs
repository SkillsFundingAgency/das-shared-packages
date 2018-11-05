using Microsoft.Azure.Documents;

namespace SFA.DAS.CosmosDb.UnitTests.Fakes
{
    public class DummyRepository : DocumentRepository<Dummy>
    {
        public DummyRepository(IDocumentClient documentClient, string databaseName, string collectionName)
            : base(documentClient, databaseName, collectionName)
        {
        }
    }
}