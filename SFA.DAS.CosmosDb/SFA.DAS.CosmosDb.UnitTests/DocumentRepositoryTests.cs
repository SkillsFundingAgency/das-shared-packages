using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Moq;
using NUnit.Framework;
using SFA.DAS.CosmosDb.Testing;
using SFA.DAS.CosmosDb.UnitTests.Fakes;
using SFA.DAS.Testing;

namespace SFA.DAS.CosmosDb.UnitTests
{
    [TestFixture]
    [Parallelizable]
    public class DocumentRepositoryTests : FluentTest<DocumentRepositoryTestsFixture>
    {
        [Test]
        public Task Add_WhenAddingDocument_ThenShouldAddDocument()
        {
            return TestAsync(f => f.Add(), f => f.DocumentClient.Verify(c => c.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(f.DatabaseName, f.CollectionName), f.Document, f.RequestOptions, true, CancellationToken.None), Times.Once));
        }

        [Test]
        public Task Add_WhenAddingDocument_ThenShouldThrowErrorIfDocumentIdIsEmpty()
        {
            return TestExceptionAsync(f => f.Add(f.DocumentWithoutId), (f,r) => r.Should().Throw<Exception>().WithMessage("Id must not be Empty"));
        }

        [Test]
        public void CreateQuery_WhenCreatingQuery_ThenShouldReturnIQueryable()
        {
            Test(f => f.SetDocuments(), f => f.CreateQuery(), (f, r) => r.Should().NotBeNull().And.BeSameAs(f.DocumentsQuery));
        }

        [Test]
        public void CreateQuery_WhenCreatingQueryWithFeedOptions_ThenShouldReturnIQueryableWithFeedOptions()
        {
            Test(f => f.SetDocuments(), f => f.CreateQueryWithFeedOptions(), (f, r) => r.Should().NotBeNull().And.BeSameAs(f.DocumentsQuery));
        }

        [Test]
        public Task GetById_WhenDocumentExists_ThenShouldReturnDocument()
        {
            return TestAsync(f => f.SetDocument(), f => f.GetById(), (f, r) => r.Should().IsSameOrEqualTo(f.Document));
        }

        [Test]
        public Task GetById_WhenDocumentExistsWithRequestOptions_ThenShouldReturnDocument()
        {
            return TestAsync(f => f.SetDocument(), f => f.GetByIdWithRequestOptions(), (f, r) => r.Should().IsSameOrEqualTo(f.Document));
        }

        [Test]
        public Task GetById_WhenDocumentDoesNotExist_ThenShouldReturnNull()
        {
            return TestAsync(f => f.SetDocumentNotFound(), f => f.GetById(), (f, r) => r.Should().BeNull());
        }

        [Test]
        public Task Remove_WhenRemovingDocument_ThenShouldRemoveDocument()
        {
            return TestAsync(f => f.Remove(), f => f.DocumentClient.Verify(c => c.DeleteDocumentAsync(UriFactory.CreateDocumentUri(f.DatabaseName, f.CollectionName, f.Document.Id.ToString()), f.RequestOptions, CancellationToken.None), Times.Once));
        }

        [Test]
        public Task Update_WhenUpdatingDocument_ThenShouldUpdateDocumentWithoutCheckingETag()
        {
            return TestAsync(f => f.Update(), f => f.DocumentClient.Verify(c =>
                c.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(f.DatabaseName, f.CollectionName, f.Document.Id.ToString()), f.Document, It.Is<RequestOptions>(o => o.AccessCondition == null), CancellationToken.None), Times.Once));
        }
        [Test]
        public Task Update_WhenUpdatingDocumentWithEmptyId_ThenShouldThrowExcepton()
        {
            return TestExceptionAsync(f => f.Update(f.DocumentWithoutId), (f, r) => r.Should().Throw<Exception>().WithMessage("Id must not be Empty"));
        }
        [Test]
        public Task Update_WhenUpdatingDocumentWithAnETag_ThenShouldUpdateDocumentCheckingETag()
        {
            return TestAsync(f => f.Update(f.DocumentWithETag), f => f.DocumentClient.Verify(c =>
                c.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(f.DatabaseName, f.CollectionName, f.DocumentWithETag.Id.ToString()), f.DocumentWithETag,
                    It.Is<RequestOptions>(o => o.AccessCondition.Type == AccessConditionType.IfMatch && o.AccessCondition.Condition == f.DocumentWithETag.ETag),
                    CancellationToken.None), Times.Once));
        }
    }

    public class DocumentRepositoryTestsFixture
    {
        public DocumentRepository<Dummy> DocumentRepository { get; set; }
        public Mock<IDocumentClient> DocumentClient { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public Dummy Document { get; set; }
        public Dummy DocumentWithoutId { get; set; }
        public Dummy DocumentWithETag { get; set; }
        public List<Dummy> Documents { get; set; }
        public IOrderedQueryable<Dummy> DocumentsQuery { get; set; }
        public FeedOptions FeedOptions { get; set; }
        public RequestOptions RequestOptions { get; set; }

        public DocumentRepositoryTestsFixture()
        {
            DocumentClient = new Mock<IDocumentClient>();
            DatabaseName = "test";
            CollectionName = "stubs";

            DocumentRepository = new DummyRepository(DocumentClient.Object, DatabaseName, CollectionName);

            Document = new Dummy {
                Id = Guid.NewGuid(),
                Name = "Test"
            };

            DocumentWithoutId = new Dummy {
                Name = "NoIdTest"
            };

            DocumentWithETag = new Dummy {
                Id = Guid.NewGuid(),
                Name = "Test",
                ETag = "ETag"
            };

            Documents = new List<Dummy>
            {
                new Dummy
                {
                    Id = Guid.NewGuid(),
                    Name = "TestA"
                },
                new Dummy
                {
                    Id = Guid.NewGuid(),
                    Name = "TestB"
                }
            };

            DocumentsQuery = Documents.AsQueryable().OrderBy(d => d.Id);
        }

        public IQueryable<Dummy> CreateQuery()
        {
            return DocumentRepository.CreateQuery();
        }

        public IQueryable<Dummy> CreateQueryWithFeedOptions()
        {
            FeedOptions = new FeedOptions();

            return DocumentRepository.CreateQuery(FeedOptions);
        }

        public Task<Dummy> GetById()
        {
            return DocumentRepository.GetById(Document.Id);
        }

        public Task<Dummy> GetByIdWithRequestOptions()
        {
            return DocumentRepository.GetById(Document.Id, RequestOptions);
        }

        public Task Add(Dummy document = null)
        {
            document = document ?? Document;
            return DocumentRepository.Add(document, RequestOptions);
        }

        public Task Remove()
        {
            return DocumentRepository.Remove(Document.Id, RequestOptions);
        }

        public Task Update(Dummy document = null)
        {
            document = document ?? Document;
            return DocumentRepository.Update(document, RequestOptions);
        }

        public DocumentRepositoryTestsFixture SetDocument()
        {
            DocumentClient.Setup(c => c.ReadDocumentAsync<Dummy>(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, Document.Id.ToString()), It.Is<RequestOptions>(r => r == RequestOptions), CancellationToken.None))
                .ReturnsAsync(new DocumentResponse<Dummy>(Document));

            return this;
        }

        public DocumentRepositoryTestsFixture SetDocumentNotFound()
        {
            DocumentClient.Setup(c => c.ReadDocumentAsync<Dummy>(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, Document.Id.ToString()), It.Is<RequestOptions>(r => r == RequestOptions), CancellationToken.None))
                .ThrowsAsync(DocumentClientExceptionBuilder.Build(new Error(), HttpStatusCode.NotFound));

            return this;
        }

        public DocumentRepositoryTestsFixture SetDocuments()
        {
            DocumentClient.Setup(c => c.CreateDocumentQuery<Dummy>(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), It.Is<FeedOptions>(f => f == FeedOptions)))
                .Returns(DocumentsQuery);

            return this;
        }
    }
}