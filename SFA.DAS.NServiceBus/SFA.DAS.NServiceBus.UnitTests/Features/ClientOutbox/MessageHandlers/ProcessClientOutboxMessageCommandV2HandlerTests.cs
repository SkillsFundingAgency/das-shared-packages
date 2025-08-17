using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
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
    public class ProcessClientOutboxMessageCommandV2HandlerTests : FluentTest<ProcessClientOutboxMessageCommandV2HandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingAProcessClientOutboxMessageCommand_ThenShouldPublishTheClientOutboxMessageEvents()
        {
            return TestAsync(f => f.Handle(), f => f.Context.PublishedMessages.Select(m => new { MessageId = Guid.Parse(m.Options.GetMessageId()), m.Message }).Should().BeEquivalentTo(f.TransportOperations));
        }

        [Test]
        public Task Handle_WhenHandlingAProcessClientOutboxMessageCommand_ThenShouldSetTheClientOutboxMessageAsDispatched()
        {
            return TestAsync(f => f.Handle(), f => f.ClientOutboxStorage.Verify(o => o.SetAsDispatchedAsync(f.ClientOutboxMessage.MessageId, f.SynchronizedStorageSession.Object)));
        }
    }

    public class ProcessClientOutboxMessageCommandV2HandlerTestsFixture
    {
        public DateTime Now { get; set; }
        public Mock<IClientOutboxStorageV2> ClientOutboxStorage { get; set; }
        public ProcessClientOutboxMessageCommandV2 Command { get; set; }
        public Mock<ISynchronizedStorageSession> SynchronizedStorageSession { get; set; }
        public TestableMessageHandlerContext Context { get; set; }
        public ProcessClientOutboxMessageCommandV2Handler Handler { get; set; }
        public ClientOutboxMessageV2 ClientOutboxMessage { get; set; }
        public string EndpointName { get; set; }
        public List<object> Events { get; set; }
        public List<TransportOperation> TransportOperations { get; set; }
        
        public ProcessClientOutboxMessageCommandV2HandlerTestsFixture()
        {
            Now = DateTime.UtcNow;
            EndpointName = "SFA.DAS.NServiceBus";

            Events = new List<object>
            {
                new FooEvent(Now.AddDays(-1)),
                new BarEvent(Now)
            };

            TransportOperations = Events.Select(e => new TransportOperation(Guid.NewGuid(), e)).ToList();
            ClientOutboxMessage = new ClientOutboxMessageV2(GuidComb.NewGuidComb(), EndpointName, TransportOperations);
            SynchronizedStorageSession = new Mock<ISynchronizedStorageSession>();

            Context = new TestableMessageHandlerContext
            {
                MessageId = ClientOutboxMessage.MessageId.ToString(),
                SynchronizedStorageSession = SynchronizedStorageSession.Object
            };

            Command = new ProcessClientOutboxMessageCommandV2();
            ClientOutboxStorage = new Mock<IClientOutboxStorageV2>();

            ClientOutboxStorage.Setup(o => o.GetAsync(ClientOutboxMessage.MessageId, SynchronizedStorageSession.Object)).ReturnsAsync(ClientOutboxMessage);

            Handler = new ProcessClientOutboxMessageCommandV2Handler(ClientOutboxStorage.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(Command, Context);
        }
    }
}