using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus.Persistence;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.NServiceBus.Features.ClientOutbox.MessageHandlers;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.Features.ClientOutbox.MessageHandlers
{
    [TestFixture]
    public class ProcessClientOutboxMessageCommandHandlerTests : FluentTest<ProcessClientOutboxMessageCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingAProcessClientOutboxMessageCommand_ThenShouldPublishTheClientOutboxMessageEvents()
        {
            return RunAsync(f => f.Handle(), f => f.Context.PublishedMessages.Select(m => m.Message).Should().BeEquivalentTo(f.Events));
        }

        [Test]
        public Task Handle_WhenHandlingAProcessClientOutboxMessageCommand_ThenShouldSetTheClientOutboxMessageAsDispatched()
        {
            return RunAsync(f => f.Handle(), f => f.ClientOutboxStorage.Verify(o => o.SetAsDispatchedAsync(f.ClientOutboxMessage.MessageId, f.SynchronizedStorageSession.Object)));
        }
    }

    public class ProcessClientOutboxMessageCommandHandlerTestsFixture
    {
        public DateTime Now { get; set; }
        public Mock<IClientOutboxStorage> ClientOutboxStorage { get; set; }
        public ProcessClientOutboxMessageCommand Command { get; set; }
        public Mock<SynchronizedStorageSession> SynchronizedStorageSession { get; set; }
        public TestableMessageHandlerContext Context { get; set; }
        public ProcessClientOutboxMessageCommandHandler Handler { get; set; }
        public ClientOutboxMessage ClientOutboxMessage { get; set; }
        public string EndpointName { get; set; }
        public List<object> Events { get; set; }
        
        public ProcessClientOutboxMessageCommandHandlerTestsFixture()
        {
            Now = DateTime.UtcNow;
            EndpointName = "SFA.DAS.NServiceBus";

            Events = new List<object>
            {
                new FooEvent(Now.AddDays(-1)),
                new BarEvent(Now)
            };

            ClientOutboxMessage = new ClientOutboxMessage(GuidComb.NewGuidComb(), EndpointName, Events);
            SynchronizedStorageSession = new Mock<SynchronizedStorageSession>();

            Context = new TestableMessageHandlerContext
            {
                MessageId = ClientOutboxMessage.MessageId.ToString(),
                SynchronizedStorageSession = SynchronizedStorageSession.Object
            };

            Command = new ProcessClientOutboxMessageCommand();
            ClientOutboxStorage = new Mock<IClientOutboxStorage>();

            ClientOutboxStorage.Setup(o => o.GetAsync(ClientOutboxMessage.MessageId, SynchronizedStorageSession.Object)).ReturnsAsync(ClientOutboxMessage);

            Handler = new ProcessClientOutboxMessageCommandHandler(ClientOutboxStorage.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(Command, Context);
        }
    }
}