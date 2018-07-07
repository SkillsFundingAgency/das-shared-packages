using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using NServiceBus.Testing;
using NUnit.Framework;

namespace SFA.DAS.NServiceBus.UnitTests
{
    [TestFixture]
    public class UnitOfWorkManagerTests : FluentTest<UnitOfWorkManagerTestsFixture>
    {
        [Test]
        public void Begin_WhenBeginningAUnitOfWork_ThenShouldBeginTransaction()
        {
            Run(f => f.Begin(), f => f.Outbox.Verify(o => o.BeginTransactionAsync(), Times.Once));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldSaveChangesBeforeAddingOutboxMessage()
        {
            Run(f => f.SetupSaveChangesBeforeAddingOutboxMessage(), f => f.BeginThenEnd(), f => f.Db.Verify(d => d.SaveChangesAsync()));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldNotSaveChanges()
        {
            Run(f => f.BeginThenEndAfterException(), f => f.Db.Verify(d => d.SaveChangesAsync(), Times.Never));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldAddOutboxMessage()
        {
            Run(f => f.SetEvents(), f => f.BeginThenEnd(), f => f.Outbox.Verify(o => o.AddAsync(It.Is<OutboxMessage>(m => !string.IsNullOrWhiteSpace(m.Data))), Times.Once));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkWithNoEvents_ThenShouldNotAddOutboxMessage()
        {
            Run(f => f.BeginThenEnd(), f => f.Outbox.Verify(o => o.AddAsync(It.IsAny<OutboxMessage>()), Times.Never));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldCommitTransaction()
        {
            Run(f => f.BeginThenEnd(), f => f.OutboxTransaction.Verify(t => t.Commit()));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldNotCommitTransaction()
        {
            Run(f => f.BeginThenEndAfterException(), f => f.OutboxTransaction.Verify(t => t.Commit(), Times.Never));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldSendProcessOutboxMessageCommand()
        {
            Run(f => f.SetEvents(), f => f.BeginThenEnd(), f => f.MessageSession.SentMessages.Should().ContainSingle(m =>
                m.Options.GetMessageId() == f.OutboxMessage.Id.ToString() &&
                m.Message.GetType() == typeof(ProcessOutboxMessageCommand)));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkWithNoEvents_ThenShouldNotSendProcessOutboxMessageCommand()
        {
            Run(f => f.BeginThenEnd(), f => f.MessageSession.SentMessages.Should().BeEmpty());
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldNotSendProcessOutboxMessageCommand()
        {
            Run(f => f.SetEvents(), f => f.BeginThenEndAfterException(), f => f.MessageSession.SentMessages.Should().BeEmpty());
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldDisposeTransaction()
        {
            Run(f => f.BeginThenEnd(), f => f.OutboxTransaction.Verify(t => t.Dispose(), Times.Once));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldDisposeTransaction()
        {
            Run(f => f.BeginThenEndAfterException(), f => f.OutboxTransaction.Verify(t => t.Dispose(), Times.Once));
        }
    }

    public class UnitOfWorkManagerTestsFixture : FluentTestFixture
    {
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }
        public Mock<IDb> Db { get; set; }
        public TestableMessageSession MessageSession { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<IOutbox> Outbox { get; set; }
        public Mock<IOutboxTransaction> OutboxTransaction { get; set; }
        public List<Event> Events { get; set; }
        public OutboxMessage OutboxMessage { get; set; }

        public UnitOfWorkManagerTestsFixture()
        {
            Db = new Mock<IDb>();
            MessageSession = new TestableMessageSession();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            Outbox = new Mock<IOutbox>();
            OutboxTransaction = new Mock<IOutboxTransaction>();

            Events = new List<Event>
            {
                new FooEvent(),
                new BarEvent()
            };
            
            Outbox.Setup(o => o.BeginTransactionAsync()).ReturnsAsync(OutboxTransaction.Object);
            Outbox.Setup(o => o.AddAsync(It.IsAny<OutboxMessage>())).Returns(Task.CompletedTask).Callback<OutboxMessage>(m => OutboxMessage = m);

            UnitOfWorkManager = new UnitOfWorkManager(
                Db.Object,
                MessageSession,
                UnitOfWorkContext.Object,
                Outbox.Object);
        }

        public void Begin()
        {
            UnitOfWorkManager.Begin();
        }

        public void BeginThenEnd()
        {
            UnitOfWorkManager.Begin();
            UnitOfWorkManager.End();
        }

        public void BeginThenEndAfterException()
        {
            UnitOfWorkManager.Begin();
            UnitOfWorkManager.End(new Exception());
        }

        public UnitOfWorkManagerTestsFixture SetEvents()
        {
            UnitOfWorkContext.Setup(c => c.GetEvents()).Returns(Events);

            return this;
        }

        public UnitOfWorkManagerTestsFixture SetupSaveChangesBeforeAddingOutboxMessage()
        {
            Db.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask).Callback(() =>
            {
                if (OutboxMessage != null)
                    throw new Exception("SaveChanges called too late");
            });

            return this;
        }

        public UnitOfWorkManagerTestsFixture SetupSaveChangesAfterAddingOutboxMessage()
        {
            var savedChanges = 0;

            Db.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask).Callback(() =>
            {
                savedChanges++;

                if (savedChanges == 2 && OutboxMessage == null)
                    throw new Exception("SaveChanges called too early");
            });

            return this;
        }

        public UnitOfWorkManagerTestsFixture SetupCommitTransactionBeforeSendingProcessOutboxMessageCommand()
        {
            OutboxTransaction.Setup(t => t.Commit()).Callback(() =>
            {
                if (MessageSession.SentMessages.Any())
                    throw new Exception("Commit called too early");
            });

            return this;
        }
    }
}