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
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Models;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork.Context;
using SFA.DAS.UnitOfWork.Pipeline;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests.Features.ClientOutbox.Pipeline
{
    [TestFixture]
    public class UnitOfWorkTests : FluentTest<UnitOfWorkTestsFixture>
    {
        [Test]
        public Task CommitAsync_WhenCommittingAUnitOfWork_ThenShouldStoreClientOutboxMessage()
        {
            return RunAsync(f => f.SetEvents(), f => f.CommitAsync(), f =>
            {
                f.ClientOutboxStorage.Verify(o => o.StoreAsync(It.IsAny<ClientOutboxMessageV2>(), f.ClientOutboxTransaction.Object), Times.Once);
                f.ClientOutboxMessage.MessageId.Should().NotBe(Guid.Empty);
                f.ClientOutboxMessage.TransportOperations.Select(o => o.MessageId).Should().NotContain(Guid.Empty);
                f.ClientOutboxMessage.TransportOperations.Select(o => o.Message).Should().BeEquivalentTo(f.Events);
            });
        }

        [Test]
        public Task CommitAsync_WhenCommittingAUnitOfWorkWithNoEvents_ThenShouldNotStoreClientOutboxMessage()
        {
            return RunAsync(f => f.CommitAsync(), f => f.ClientOutboxStorage.Verify(o => o.StoreAsync(It.IsAny<ClientOutboxMessageV2>(), It.IsAny<IClientOutboxTransaction>()), Times.Never));
        }

        [Test]
        public Task CommitAsync_WhenCommittingAUnitOfWork_ThenShouldPublishEvents()
        {
            return RunAsync(
                f => f.SetEvents(), 
                f => f.CommitAsync(), 
                f => f.MessageSession.PublishedMessages.Select(m => new { MessageId = Guid.Parse(m.Options.GetMessageId()), m.Message }).Should().BeEquivalentTo(f.ClientOutboxMessage.TransportOperations));
        }

        [Test]
        public Task CommitAsync_WhenCommittingAUnitOfWork_ThenShouldPublishCommands()
        {
            return RunAsync(
                f => f.SetCommands(),
                f => f.CommitAsync(),
                f => f.MessageSession.SentMessages.Select(m => new { MessageId = Guid.Parse(m.Options.GetMessageId()), m.Message }).Should().BeEquivalentTo(f.ClientOutboxMessage.TransportOperations));
        }

        [Test]
        public Task CommitAsync_WhenCommittingAUnitOfWork_ThenShouldSetTheClientOutboxMessageAsDispatched()
        {
            return RunAsync(f => f.SetEvents(), f => f.CommitAsync(), f => f.ClientOutboxStorage.Verify(o => o.SetAsDispatchedAsync(f.ClientOutboxMessage.MessageId, It.IsAny<IClientOutboxTransaction>())));
        }

        [Test]
        public Task CommitAsync_WhenCommittingAUnitOfWorkWithNoEvents_ThenShouldNotPublishEvents()
        {
            return RunAsync(f => f.CommitAsync(), f => f.MessageSession.PublishedMessages.Should().BeEmpty());
        }
    }

    public class UnitOfWorkTestsFixture
    {
        public IUnitOfWork UnitOfWork { get; set; }
        public Mock<IClientOutboxStorageV2> ClientOutboxStorage { get; set; }
        public TestableMessageSession MessageSession { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<ReadOnlySettings> Settings { get; set; }
        public Mock<IClientOutboxTransaction> ClientOutboxTransaction { get; set; }
        public Mock<Func<Task>> NextTask { get; set; }
        public bool NextTaskInvoked { get; set; }
        public string EndpointName { get; set; }
        public List<object> Events { get; set; }
        public List<object> Commands { get; set; }
        public ClientOutboxMessageV2 ClientOutboxMessage { get; set; }

        public UnitOfWorkTestsFixture()
        {
            ClientOutboxStorage = new Mock<IClientOutboxStorageV2>();
            MessageSession = new TestableMessageSession();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            Settings = new Mock<ReadOnlySettings>();
            ClientOutboxTransaction = new Mock<IClientOutboxTransaction>();
            EndpointName = "SFA.DAS.NServiceBus";
            NextTask = new Mock<Func<Task>>();

            Events = new List<object>
            {
                new FooEvent(DateTime.UtcNow),
                new BarEvent(DateTime.UtcNow)
            };

            Commands = new List<object>
            {
                new TestCommand(DateTime.UtcNow),
                new AnotherCommand(DateTime.UtcNow),
                new TestCommand(DateTime.UtcNow)
            };

            UnitOfWorkContext.Setup(c => c.Get<IClientOutboxTransaction>()).Returns(ClientOutboxTransaction.Object);
            Settings.Setup(s => s.Get<string>("NServiceBus.Routing.EndpointName")).Returns(EndpointName);

            ClientOutboxStorage.Setup(o => o.StoreAsync(It.IsAny<ClientOutboxMessageV2>(), It.IsAny<IClientOutboxTransaction>()))
                .Returns(Task.CompletedTask).Callback<ClientOutboxMessageV2, IClientOutboxTransaction>((m, t) =>
                {
                    if (NextTaskInvoked)
                        throw new Exception("StoreAsync called too late");

                    ClientOutboxMessage = m;
                });

            NextTask.Setup(n => n()).Returns(Task.CompletedTask).Callback(() =>
            {
                if (MessageSession.PublishedMessages.Any())
                    throw new Exception("Publish called too early");

                NextTaskInvoked = true;
            });

            UnitOfWork = new NServiceBus.Features.ClientOutbox.Pipeline.UnitOfWork(
                ClientOutboxStorage.Object,
                MessageSession,
                UnitOfWorkContext.Object,
                Settings.Object);
        }

        public Task CommitAsync()
        {
            return UnitOfWork.CommitAsync(NextTask.Object);
        }

        public UnitOfWorkTestsFixture SetEvents()
        {
            UnitOfWorkContext.Setup(c => c.GetEvents()).Returns(Events);

            return this;
        }

        public UnitOfWorkTestsFixture SetCommands()
        {
            UnitOfWorkContext.Setup(c => c.GetEvents()).Returns(Commands);

            return this;
        }
    }
}