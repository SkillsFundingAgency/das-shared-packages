using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.ClientOutbox
{
    [TestFixture]
    public class ProcessClientOutboxMessageCommandHandlerTests : FluentTest<ProcessClientOutboxMessageCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingAProcessOutboxMessageCommand_ThenShouldPublishTheOutboxMessageEvents()
        {
            return RunAsync(f => f.Handle(), f => f.Context.PublishedMessages.Select(m => m.Message).Cast<Event>().Should().BeEquivalentTo(f.Events));
        }

        [Test]
        public Task Handle_WhenHandlingAProcessOutboxMessageCommand_ThenShouldSetTheOutboxMessageAsDispatched()
        {
            return RunAsync(f => f.Handle(), f => f.ClientOutboxStorage.Verify(o => o.SetAsDispatchedAsync(f.ClientOutboxMessage.MessageId, null)));
        }
    }

    public class ProcessClientOutboxMessageCommandHandlerTestsFixture : FluentTestFixture
    {
        public DateTime Now { get; set; }
        public Mock<IClientOutboxStorage> ClientOutboxStorage { get; set; }
        public ProcessClientOutboxMessageCommand Command { get; set; }
        public TestableMessageHandlerContext Context { get; set; }
        public ProcessClientOutboxMessageCommandHandler Handler { get; set; }
        public ClientOutboxMessage ClientOutboxMessage { get; set; }
        public string EndpointName { get; set; }
        public List<Event> Events { get; set; }
        
        public ProcessClientOutboxMessageCommandHandlerTestsFixture()
        {
            Now = DateTime.UtcNow;

            EndpointName = "SFA.DAS.NServiceBus";

            Events = new List<Event>
            {
                new FooEvent { Created = Now.AddDays(-1) },
                new BarEvent { Created = Now }
            };

            ClientOutboxMessage = new ClientOutboxMessage(GuidComb.NewGuidComb(), EndpointName, Events);

            Context = new TestableMessageHandlerContext
            {
                MessageId = ClientOutboxMessage.MessageId.ToString()
            };

            Command = new ProcessClientOutboxMessageCommand();
            ClientOutboxStorage = new Mock<IClientOutboxStorage>();

            ClientOutboxStorage.Setup(o => o.GetAsync(ClientOutboxMessage.MessageId, null)).ReturnsAsync(ClientOutboxMessage);

            Handler = new ProcessClientOutboxMessageCommandHandler(ClientOutboxStorage.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(Command, Context);
        }
    }
}