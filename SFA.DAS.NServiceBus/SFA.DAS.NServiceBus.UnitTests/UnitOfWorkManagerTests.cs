using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using NServiceBus.Settings;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.Testing;

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
        public void End_WhenEndingAUnitOfWork_ThenShouldStoreOutboxMessage()
        {
            Run(f => f.SetEvents(), f => f.BeginThenEnd(), f =>
            {
                f.Outbox.Verify(o => o.StoreAsync(It.Is<OutboxMessage>(m => m.MessageId != Guid.Empty), f.OutboxTransaction.Object), Times.Once);
                f.OutboxMessage.Operations.Should().BeEquivalentTo(f.Events);
            });
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkWithNoEvents_ThenShouldNotStoreOutboxMessage()
        {
            Run(f => f.BeginThenEnd(), f => f.Outbox.Verify(o => o.StoreAsync(It.IsAny<OutboxMessage>(), It.IsAny<IOutboxTransaction>()), Times.Never));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldCommitTransaction()
        {
            Run(f => f.BeginThenEnd(), f => f.OutboxTransaction.Verify(t => t.CommitAsync()));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldNotCommitTransaction()
        {
            Run(f => f.BeginThenEndAfterException(), f => f.OutboxTransaction.Verify(t => t.CommitAsync(), Times.Never));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldSendProcessOutboxMessageCommand()
        {
            Run(f => f.SetEvents(), f => f.BeginThenEnd(), f => f.MessageSession.SentMessages.Should().ContainSingle(m =>
                m.Options.GetMessageId() == f.OutboxMessage.MessageId.ToString() &&
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
        public Mock<ReadOnlySettings> Settings { get; set; }
        public Mock<IOutboxTransaction> OutboxTransaction { get; set; }
        public string EndpointName { get; set; }
        public List<Event> Events { get; set; }
        public OutboxMessage OutboxMessage { get; set; }

        public UnitOfWorkManagerTestsFixture()
        {
            Db = new Mock<IDb>();
            MessageSession = new TestableMessageSession();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            Outbox = new Mock<IOutbox>();
            Settings = new Mock<ReadOnlySettings>();
            OutboxTransaction = new Mock<IOutboxTransaction>();
            EndpointName = "SFA.DAS.NServiceBus";

            Events = new List<Event>
            {
                new FooEvent(),
                new BarEvent()
            };
            
            Outbox.Setup(o => o.BeginTransactionAsync()).ReturnsAsync(OutboxTransaction.Object);

            Outbox.Setup(o => o.StoreAsync(It.IsAny<OutboxMessage>(), It.IsAny<IOutboxTransaction>()))
                .Returns(Task.CompletedTask).Callback<OutboxMessage, IOutboxTransaction>((m, t) => OutboxMessage = m);

            Settings.Setup(s => s.Get<string>("NServiceBus.Routing.EndpointName")).Returns(EndpointName);

            UnitOfWorkManager = new UnitOfWorkManager(
                Db.Object,
                MessageSession,
                UnitOfWorkContext.Object,
                Outbox.Object,
                Settings.Object);
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
            OutboxTransaction.Setup(t => t.CommitAsync()).Callback(() =>
            {
                if (MessageSession.SentMessages.Any())
                    throw new Exception("Commit called too early");
            });

            return this;
        }
    }
}