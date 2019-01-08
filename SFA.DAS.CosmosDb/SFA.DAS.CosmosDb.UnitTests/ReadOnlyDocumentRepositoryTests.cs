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
    public class ReadOnlyDocumentRepositoryTests : FluentTest<ReadOnlyDocumentRepositoryTestsFixture>
    {
        [Test]
        public void CreateQuery_WhenCreatingQuery_ThenShouldReturnIQueryable()
        {
            Run(f => f.SetDocuments(), f => f.CreateQuery(), (f, r) => r.Should().NotBeNull().And.BeSameAs(f.DocumentsQuery));
        }

        [Test]
        public void CreateQuery_WhenCreatingQueryWithFeedOptions_ThenShouldReturnIQueryableWithFeedOptions()
        {
            Run(f => f.SetDocuments(), f => f.CreateQueryWithFeedOptions(), (f, r) => r.Should().NotBeNull().And.BeSameAs(f.DocumentsQuery));
        }

        [Test]
        public Task GetById_WhenDocumentExists_ThenShouldReturnDocument()
        {
            return RunAsync(f => f.SetDocument(), f => f.GetById(), (f, r) => r.Should().IsSameOrEqualTo(f.Document));
        }

        [Test]
        public Task GetById_WhenDocumentExistsWithRequestOptions_ThenShouldReturnDocument()
        {
            return RunAsync(f => f.SetDocument(), f => f.GetByIdWithRequestOptions(), (f, r) => r.Should().IsSameOrEqualTo(f.Document));
        }

        [Test]
        public Task GetById_WhenDocumentDoesNotExist_ThenShouldReturnNull()
        {
            return RunAsync(f => f.SetDocumentNotFound(), f => f.GetById(), (f, r) => r.Should().BeNull());
        }
    }

    public class ReadOnlyDocumentRepositoryTestsFixture
    {
        public ReadOnlyDocumentRepository<ReadOnlyDummy> ReadOnlyDocumentRepository { get; set; }
        public Mock<IDocumentClient> DocumentClient { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public ReadOnlyDummy Document { get; set; }
        public Guid DocumentId = Guid.NewGuid();
        public List<ReadOnlyDummy> Documents { get; set; }
        public IOrderedQueryable<ReadOnlyDummy> DocumentsQuery { get; set; }
        public FeedOptions FeedOptions { get; set; }
        public RequestOptions RequestOptions { get; set; }

        public ReadOnlyDocumentRepositoryTestsFixture()
        {
            DocumentClient = new Mock<IDocumentClient>();
            DatabaseName = "test";
            CollectionName = "stubs";

            ReadOnlyDocumentRepository = new ReadOnlyDummyRepository(DocumentClient.Object, DatabaseName, CollectionName);

            Document = new ReadOnlyDummy { Name = "Test" };

            Documents = new List<ReadOnlyDummy>
            {
                new ReadOnlyDummy
                {
                    Name = "TestA"
                },
                new ReadOnlyDummy
                {
                    Name = "TestB"
                }
            };

            DocumentsQuery = Documents.AsQueryable().OrderBy(d => d.Name);
        }

        public IQueryable<ReadOnlyDummy> CreateQuery()
        {
            return ReadOnlyDocumentRepository.CreateQuery();
        }

        public IQueryable<ReadOnlyDummy> CreateQueryWithFeedOptions()
        {
            FeedOptions = new FeedOptions();

            return ReadOnlyDocumentRepository.CreateQuery(FeedOptions);
        }

        public Task<ReadOnlyDummy> GetById()
        {
            return ReadOnlyDocumentRepository.GetById(DocumentId);
        }

        public Task<ReadOnlyDummy> GetByIdWithRequestOptions()
        {
            return ReadOnlyDocumentRepository.GetById(DocumentId, RequestOptions);
        }

        public ReadOnlyDocumentRepositoryTestsFixture SetDocument()
        {
            DocumentClient.Setup(c => c.ReadDocumentAsync<ReadOnlyDummy>(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, DocumentId.ToString()), It.Is<RequestOptions>(r => r == RequestOptions), CancellationToken.None))
                .ReturnsAsync(new DocumentResponse<ReadOnlyDummy>(Document));

            return this;
        }

        public ReadOnlyDocumentRepositoryTestsFixture SetDocumentNotFound()
        {
            DocumentClient.Setup(c => c.ReadDocumentAsync<ReadOnlyDummy>(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, DocumentId.ToString()), It.Is<RequestOptions>(r => r == RequestOptions), CancellationToken.None))
                .ThrowsAsync(DocumentClientExceptionBuilder.Build(new Error(), HttpStatusCode.NotFound));

            return this;
        }

        public ReadOnlyDocumentRepositoryTestsFixture SetDocuments()
        {
            DocumentClient.Setup(c => c.CreateDocumentQuery<ReadOnlyDummy>(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), It.Is<FeedOptions>(f => f == FeedOptions)))
                .Returns(DocumentsQuery);

            return this;
        }
    }
}