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
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.NServiceBus.ClientOutbox.UnitOfWork;
using SFA.DAS.NServiceBus.UnitOfWork;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.ClientOutbox.UnitOfWork
{
    [TestFixture]
    public class UnitOfWorkManagerTests : FluentTest<UnitOfWorkManagerTestsFixture>
    {
        [Test]
        public void Begin_WhenBeginningAUnitOfWork_ThenShouldBeginTransaction()
        {
            Run(f => f.Begin(), f => f.ClientOutboxStorage.Verify(o => o.BeginTransactionAsync(), Times.Once));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldSaveChangesBeforeAddingClientOutboxMessage()
        {
            Run(f => f.SetupSaveChangesBeforeAddingClientOutboxMessage(), f => f.BeginThenEnd(), f => f.Db.Verify(d => d.SaveChangesAsync()));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldNotSaveChanges()
        {
            Run(f => f.BeginThenEndAfterException(), f => f.Db.Verify(d => d.SaveChangesAsync(), Times.Never));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldStoreClientOutboxMessage()
        {
            Run(f => f.SetEvents(), f => f.BeginThenEnd(), f =>
            {
                f.ClientOutboxStorage.Verify(o => o.StoreAsync(It.Is<ClientOutboxMessage>(m => m.MessageId != Guid.Empty), f.ClientOutboxTransaction.Object), Times.Once);
                f.ClientOutboxMessage.Operations.Should().BeEquivalentTo(f.Events);
            });
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkWithNoEvents_ThenShouldNotStoreClientOutboxMessage()
        {
            Run(f => f.BeginThenEnd(), f => f.ClientOutboxStorage.Verify(o => o.StoreAsync(It.IsAny<ClientOutboxMessage>(), It.IsAny<IClientOutboxTransaction>()), Times.Never));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldCommitTransaction()
        {
            Run(f => f.BeginThenEnd(), f => f.ClientOutboxTransaction.Verify(t => t.CommitAsync()));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldNotCommitTransaction()
        {
            Run(f => f.BeginThenEndAfterException(), f => f.ClientOutboxTransaction.Verify(t => t.CommitAsync(), Times.Never));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldSendProcessClientOutboxMessageCommand()
        {
            Run(f => f.SetEvents(), f => f.BeginThenEnd(), f => f.MessageSession.SentMessages.Should().ContainSingle(m =>
                m.Options.GetMessageId() == f.ClientOutboxMessage.MessageId.ToString() &&
                m.Message.GetType() == typeof(ProcessClientOutboxMessageCommand)));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkWithNoEvents_ThenShouldNotSendProcessClientOutboxMessageCommand()
        {
            Run(f => f.BeginThenEnd(), f => f.MessageSession.SentMessages.Should().BeEmpty());
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldNotSendProcessClientOutboxMessageCommand()
        {
            Run(f => f.SetEvents(), f => f.BeginThenEndAfterException(), f => f.MessageSession.SentMessages.Should().BeEmpty());
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldDisposeTransaction()
        {
            Run(f => f.BeginThenEnd(), f => f.ClientOutboxTransaction.Verify(t => t.Dispose(), Times.Once));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldDisposeTransaction()
        {
            Run(f => f.BeginThenEndAfterException(), f => f.ClientOutboxTransaction.Verify(t => t.Dispose(), Times.Once));
        }
    }

    public class UnitOfWorkManagerTestsFixture : FluentTestFixture
    {
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }
        public Mock<IDb> Db { get; set; }
        public TestableMessageSession MessageSession { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<IClientOutboxStorage> ClientOutboxStorage { get; set; }
        public Mock<ReadOnlySettings> Settings { get; set; }
        public Mock<IClientOutboxTransaction> ClientOutboxTransaction { get; set; }
        public string EndpointName { get; set; }
        public List<Event> Events { get; set; }
        public ClientOutboxMessage ClientOutboxMessage { get; set; }

        public UnitOfWorkManagerTestsFixture()
        {
            Db = new Mock<IDb>();
            MessageSession = new TestableMessageSession();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            ClientOutboxStorage = new Mock<IClientOutboxStorage>();
            Settings = new Mock<ReadOnlySettings>();
            ClientOutboxTransaction = new Mock<IClientOutboxTransaction>();
            EndpointName = "SFA.DAS.NServiceBus";

            Events = new List<Event>
            {
                new FooEvent(),
                new BarEvent()
            };
            
            ClientOutboxStorage.Setup(o => o.BeginTransactionAsync()).ReturnsAsync(ClientOutboxTransaction.Object);

            ClientOutboxStorage.Setup(o => o.StoreAsync(It.IsAny<ClientOutboxMessage>(), It.IsAny<IClientOutboxTransaction>()))
                .Returns(Task.CompletedTask).Callback<ClientOutboxMessage, IClientOutboxTransaction>((m, t) => ClientOutboxMessage = m);

            Settings.Setup(s => s.Get<string>("NServiceBus.Routing.EndpointName")).Returns(EndpointName);

            UnitOfWorkManager = new UnitOfWorkManager(
                Db.Object,
                MessageSession,
                UnitOfWorkContext.Object,
                ClientOutboxStorage.Object,
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

        public UnitOfWorkManagerTestsFixture SetupSaveChangesBeforeAddingClientOutboxMessage()
        {
            Db.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask).Callback(() =>
            {
                if (ClientOutboxMessage != null)
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

                if (savedChanges == 2 && ClientOutboxMessage == null)
                    throw new Exception("SaveChanges called too early");
            });

            return this;
        }

        public UnitOfWorkManagerTestsFixture SetupCommitTransactionBeforeSendingProcessOutboxMessageCommand()
        {
            ClientOutboxTransaction.Setup(t => t.CommitAsync()).Callback(() =>
            {
                if (MessageSession.SentMessages.Any())
                    throw new Exception("Commit called too early");
            });

            return this;
        }
    }
}