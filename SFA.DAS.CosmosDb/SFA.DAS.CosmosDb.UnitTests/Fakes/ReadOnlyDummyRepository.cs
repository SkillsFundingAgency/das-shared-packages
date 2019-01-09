using Microsoft.Azure.Documents;

namespace SFA.DAS.CosmosDb.UnitTests.Fakes
{
    public class ReadOnlyDummyRepository : ReadOnlyDocumentRepository<ReadOnlyDummy>
    {
        public ReadOnlyDummyRepository(IDocumentClient documentClient, string databaseName, string collectionName)
            : base(documentClient, databaseName, collectionName)
        {
        }
    }
}