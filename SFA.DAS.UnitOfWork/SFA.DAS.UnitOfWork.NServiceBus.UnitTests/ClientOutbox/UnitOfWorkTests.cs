using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using NServiceBus.Settings;
using NServiceBus.Testing;
using NServiceBus.UniformSession;
using NUnit.Framework;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.NServiceBus.ClientOutbox.Commands;
using SFA.DAS.Testing;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests.ClientOutbox
{
    [TestFixture]
    public class UnitOfWorkTests : FluentTest<UnitOfWorkTestsFixture>
    {
        [Test]
        public Task CommitAsync_WhenCommittingAUnitOfWork_ThenShouldStoreClientOutboxMessage()
        {
            return RunAsync(f => f.SetEvents(), f => f.CommitAsync(), f =>
            {
                f.ClientOutboxStorage.Verify(o => o.StoreAsync(It.Is<ClientOutboxMessage>(m => m.MessageId != Guid.Empty), f.ClientOutboxTransaction.Object), Times.Once);
                f.ClientOutboxMessage.Operations.Should().BeEquivalentTo(f.Events);
            });
        }

        [Test]
        public Task CommitAsync_WhenCommittingAUnitOfWorkWithNoEvents_ThenShouldNotStoreClientOutboxMessage()
        {
            return RunAsync(f => f.CommitAsync(), f => f.ClientOutboxStorage.Verify(o => o.StoreAsync(It.IsAny<ClientOutboxMessage>(), It.IsAny<IClientOutboxTransaction>()), Times.Never));
        }

        [Test]
        public Task CommitAsync_WhenCommittingAUnitOfWork_ThenShouldSendProcessClientOutboxMessageCommand()
        {
            return RunAsync(f => f.SetEvents(), f => f.CommitAsync(), f => f.MessageSession.SentMessages.Should().ContainSingle(m =>
                m.Options.GetMessageId() == f.ClientOutboxMessage.MessageId.ToString() &&
                m.Message.GetType() == typeof(ProcessClientOutboxMessageCommand)));
        }

        [Test]
        public Task CommitAsync_WhenCommittingAUnitOfWorkWithNoEvents_ThenShouldNotSendProcessClientOutboxMessageCommand()
        {
            return RunAsync(f => f.CommitAsync(), f => f.MessageSession.SentMessages.Should().BeEmpty());
        }
    }

    public class UnitOfWorkTestsFixture
    {
        public IUnitOfWork UnitOfWork { get; set; }
        public Mock<IClientOutboxStorage> ClientOutboxStorage { get; set; }
        public Mock<IUniformSession> UniformSession { get; set; }
        public TestableMessageSession MessageSession { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<ReadOnlySettings> Settings { get; set; }
        public Mock<IClientOutboxTransaction> ClientOutboxTransaction { get; set; }
        public Mock<Func<Task>> NextTask { get; set; }
        public bool NextTaskInvoked { get; set; }
        public string EndpointName { get; set; }
        public List<object> Events { get; set; }
        public ClientOutboxMessage ClientOutboxMessage { get; set; }

        public UnitOfWorkTestsFixture()
        {
            ClientOutboxStorage = new Mock<IClientOutboxStorage>();
            UniformSession = new Mock<IUniformSession>();
            MessageSession = new TestableMessageSession();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            Settings = new Mock<ReadOnlySettings>();
            ClientOutboxTransaction = new Mock<IClientOutboxTransaction>();
            NextTask = new Mock<Func<Task>>();
            EndpointName = "SFA.DAS.NServiceBus";

            Events = new List<object>
            {
                new FooEvent(DateTime.UtcNow),
                new BarEvent(DateTime.UtcNow)
            };

            UnitOfWorkContext.Setup(c => c.Get<IClientOutboxTransaction>()).Returns(ClientOutboxTransaction.Object);

            ClientOutboxStorage.Setup(o => o.StoreAsync(It.IsAny<ClientOutboxMessage>(), It.IsAny<IClientOutboxTransaction>()))
                .Returns(Task.CompletedTask).Callback<ClientOutboxMessage, IClientOutboxTransaction>((m, t) =>
                {
                    if (NextTaskInvoked)
                        throw new Exception("StoreAsync called too late");

                    ClientOutboxMessage = m;
                });

            UniformSession.Setup(s => s.Send(It.IsAny<object>(), It.IsAny<SendOptions>()))
                .Returns(Task.CompletedTask).Callback<object, SendOptions>((m, o) =>
                {
                    if (!NextTaskInvoked)
                        throw new Exception("Send called too early");

                    MessageSession.Send(m, o);
                });

            NextTask.Setup(n => n()).Returns(Task.CompletedTask).Callback(() => NextTaskInvoked = true);

            Settings.Setup(s => s.Get<string>("NServiceBus.Routing.EndpointName")).Returns(EndpointName);

            UnitOfWork = new NServiceBus.ClientOutbox.UnitOfWork(
                ClientOutboxStorage.Object,
                UniformSession.Object,
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
    }
}