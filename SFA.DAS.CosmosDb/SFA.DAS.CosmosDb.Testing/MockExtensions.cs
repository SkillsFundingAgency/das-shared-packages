using System.Collections.Generic;
using Moq;

namespace SFA.DAS.CosmosDb.Testing
{
    public static class MockExtensions
    {
        public static void SetupInMemoryCollection<TRepository, TDocument>(this Mock<TRepository> documentRepository, IEnumerable<TDocument> documents) where TRepository : class, IDocumentRepository<TDocument> where TDocument : class
        {
            var documentQuery = new FakeDocumentQuery<TDocument>(documents);

            documentRepository.Setup(r => r.CreateQuery(null)).Returns(documentQuery);
        }
    }
}