using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.NServiceBus.EntityFramework;

namespace SFA.DAS.NServiceBus.UnitTests.EntityFramework
{
    [TestFixture]
    public class OutboxTests : FluentTest<OutboxTestsFixture>
    {
        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldOpenConnection()
        {
            return RunAsync(f => f.BeginTransactionAsync(), f => f.Connection.Verify(c => c.OpenAsync(CancellationToken.None), Times.Once()));
        }

        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldBeginTransaction()
        {
            return RunAsync(f => f.BeginTransactionAsync(), f => f.Connection.Protected().Verify<DbTransaction>("BeginDbTransaction", Times.Once(), IsolationLevel.Unspecified));
        }

        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldSetUnitOfWorkContextDbConnection()
        {
            return RunAsync(f => f.BeginTransactionAsync(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.Connection.Object), Times.Once()));
        }

        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldSetUnitOfWorkContextDbTransaction()
        {
            return RunAsync(f => f.BeginTransactionAsync(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.Transaction.Object), Times.Once()));
        }

        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldReturnOutboxTransaction()
        {
            return RunAsync(f => f.BeginTransactionAsync(), (f, r) => r.Should().NotBeNull());
        }

        [Test]
        public Task AddAsync_WhenAddingAnOutboxMessage_ThenShouldAddOutboxMessage()
        {
            return RunAsync(f => f.SetupOutboxMessages(), f => f.AddAsync(), f => f.Db.Verify(d => d.OutboxMessages.Add(f.OutboxMessage), Times.Once));
        }

        [Test]
        public Task AddAsync_WhenAddingAnOutboxMessage_ThenShouldSaveChanges()
        {
            return RunAsync(f => f.SetupOutboxMessages(), f => f.AddAsync(), f => f.Db.Verify(d => d.SaveChangesAsync()));
        }

        [Test]
        public Task GetById_WhenGettingById_TheShouldReturnOutboxMessage()
        {
            return RunAsync(f => f.SetOutboxMessages(), f => f.GetById(), (f, r) => r.Should().Be(f.OutboxMessage));
        }

        [Test]
        public Task GetIdsToProcess_WhenGettingIdsToProcess_TheShouldReturnIdsOfUnpublishedOutboxMessagesCreatedMoreThan10MinutesAgo()
        {
            return RunAsync(f => f.SetOutboxMessages(), f => f.GetIdsToProcess(), (f, r) => r.Should().HaveCount(2).And
                .ContainInOrder(f.OutboxMessagesToProcess.Select(m => m.Id)));
        }
    }

    public class OutboxTestsFixture : FluentTestFixture
    {
        public DateTime Now { get; set; }
        public IOutbox Outbox { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<DbConnection> Connection { get; set; }
        public Mock<DbContextFake> Db { get; set; }
        public Mock<DbTransaction> Transaction { get; set; }
        public OutboxMessage OutboxMessage { get; set; }
        public OutboxMessage OutboxMessageToProcess1 { get; set; }
        public OutboxMessage OutboxMessageToProcess2 { get; set; }
        public OutboxMessage PublishedOutboxMessage { get; set; }
        public List<OutboxMessage> OutboxMessages { get; set; }
        public List<OutboxMessage> OutboxMessagesToProcess { get; set; }

        public OutboxTestsFixture()
        {
            Now = DateTime.UtcNow;
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            Connection = new Mock<DbConnection>();
            Db = new Mock<DbContextFake>();
            Transaction = new Mock<DbTransaction> { CallBase = true };
            PublishedOutboxMessage = new OutboxMessageBuilder().WithId(GuidComb.NewGuidComb()).WithSent(Now.AddMinutes(-30)).WithPublished(Now).Build();
            OutboxMessageToProcess1 = new OutboxMessageBuilder().WithId(GuidComb.NewGuidComb()).WithSent(Now.AddMinutes(-20)).Build();
            OutboxMessageToProcess2 = new OutboxMessageBuilder().WithId(GuidComb.NewGuidComb()).WithSent(Now.AddMinutes(-10)).Build();
            OutboxMessage = new OutboxMessageBuilder().WithId(GuidComb.NewGuidComb()).WithSent(Now).Build();
            OutboxMessages = new List<OutboxMessage>();

            OutboxMessagesToProcess = new List<OutboxMessage>
            {
                OutboxMessageToProcess1,
                OutboxMessageToProcess2
            };

            Connection.Protected().Setup<DbTransaction>("BeginDbTransaction", IsolationLevel.Unspecified).Returns(Transaction.Object);
            Db.Setup(d => d.OutboxMessages).Returns(new DbSetStub<OutboxMessage>(OutboxMessages));
            Db.Setup(d => d.SaveChangesAsync()).ReturnsAsync(1);

            Outbox = new Outbox<DbContextFake>(
                UnitOfWorkContext.Object,
                Connection.Object,
                new Lazy<DbContextFake>(() => Db.Object));
        }

        public Task<IOutboxTransaction> BeginTransactionAsync()
        {
            return Outbox.BeginTransactionAsync();
        }

        public Task AddAsync()
        {
            return Outbox.AddAsync(OutboxMessage);
        }

        public Task<OutboxMessage> GetById()
        {
            return Outbox.GetById(OutboxMessage.Id);
        }

        public Task<IEnumerable<Guid>> GetIdsToProcess()
        {
            return Outbox.GetIdsToProcess();
        }

        public OutboxTestsFixture SetupOutboxMessages()
        {
            Db.Setup(d => d.OutboxMessages.Add(It.IsAny<OutboxMessage>()));

            return this;
        }

        public OutboxTestsFixture SetOutboxMessages()
        {
            OutboxMessages.Add(OutboxMessage);
            OutboxMessages.Add(OutboxMessageToProcess2);
            OutboxMessages.Add(OutboxMessageToProcess1);
            OutboxMessages.Add(PublishedOutboxMessage);

            return this;
        }
    }
}