using System.Data.Common;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.NServiceBus.MsSqlServer;

namespace SFA.DAS.NServiceBus.UnitTests.MsSqlServer
{
    [TestFixture]
    public class OutboxTransactionTests : FluentTest<OutboxTransactionTestsFixture>
    {
        [Test]
        public void New_WhenCreatingAnOutboxTransaction_ThenShouldCreateOutboxTransaction()
        {
            Run(f => f.New(), (f, r) => r.Should().NotBeNull());
        }

        [Test]
        public void Commit_WhenCommitting_ThenShouldCommitTransaction()
        {
            Run(f => f.Commit(), f => f.Transaction.Verify(t => t.Commit(), Times.Once));
        }

        [Test]
        public void Dispose_WhenDisposing_ThenShouldDisposeTransaction()
        {
            Run(f => f.Dispose(), f => f.Transaction.Protected().Verify("Dispose", Times.Once(), true));
        }
    }

    public class OutboxTransactionTestsFixture : FluentTestFixture
    {
        public Mock<DbTransaction> Transaction { get; set; }

        public OutboxTransactionTestsFixture()
        {
            Transaction = new Mock<DbTransaction>();
        }

        public OutboxTransaction New()
        {
            return new OutboxTransaction(Transaction.Object);
        }

        public void Commit()
        {
            var outboxTransaction = New();

            outboxTransaction.Commit();
        }

        public void Dispose()
        {
            var outboxTransaction = New();

            outboxTransaction.Dispose();
        }
    }
}