using System.Data.Common;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.NServiceBus.MsSqlServer;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.MsSqlServer
{
    [TestFixture]
    public class OutboxTransactionTests : FluentTest<OutboxTransactionTestsFixture>
    {
        [Test]
        public void New_WhenCreatingAnOutboxTransaction_ThenShouldCreateAnOutboxTransaction()
        {
            Run(f => f.New(), (f, r) => r.Should().NotBeNull());
        }
        [Test]
        public void Transaction_WhenGettingTheTransaction_ThenShouldReturnTheTransaction()
        {
            Run(f => f.SetOutboxTransaction(), f => f.OutboxTransaction.Transaction, (f, r) => r.Should().Be(f.Transaction.Object));
        }

        [Test]
        public Task CommitAsync_WhenCommitting_ThenShouldCommitTransaction()
        {
            return RunAsync(f => f.SetOutboxTransaction(), f => f.CommitAsync(), f => f.Transaction.Verify(t => t.Commit(), Times.Once));
        }

        [Test]
        public void Dispose_WhenDisposing_ThenShouldDisposeTheTransaction()
        {
            Run(f => f.SetOutboxTransaction(), f => f.Dispose(), f => f.Transaction.Protected().Verify("Dispose", Times.Once(), true));
        }
    }

    public class OutboxTransactionTestsFixture : FluentTestFixture
    {
        public Mock<DbTransaction> Transaction { get; set; }
        public OutboxTransaction OutboxTransaction { get; set; }

        public OutboxTransactionTestsFixture()
        {
            Transaction = new Mock<DbTransaction>();
        }

        public OutboxTransaction New()
        {
            return new OutboxTransaction(Transaction.Object);
        }

        public Task CommitAsync()
        {
            return OutboxTransaction.CommitAsync();
        }

        public void Dispose()
        {
            OutboxTransaction.Dispose();
        }

        public OutboxTransactionTestsFixture SetOutboxTransaction()
        {
            OutboxTransaction = New();

            return this;
        }
    }
}