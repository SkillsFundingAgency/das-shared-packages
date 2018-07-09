using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests
{
    [TestFixture]
    public class ProcessOutboxMessageCommandHandlerTests : FluentTest<ProcessOutboxMessageCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingAProcessOutboxMessageCommand_ThenShouldPublishTheOutboxMessageEvents()
        {
            return RunAsync(f => f.Handle(), f => f.Context.PublishedMessages.Select(m => m.Message).Cast<Event>().Should().BeEquivalentTo(f.Events));
        }

        [Test]
        public Task Handle_WhenHandlingAProcessOutboxMessageCommand_ThenShouldSetTheOutboxMessageAsDispatched()
        {
            return RunAsync(f => f.Handle(), f => f.Outbox.Verify(o => o.SetAsDispatchedAsync(f.OutboxMessage.MessageId)));
        }
    }

    public class ProcessOutboxMessageCommandHandlerTestsFixture : FluentTestFixture
    {
        public DateTime Now { get; set; }
        public Mock<IOutbox> Outbox { get; set; }
        public ProcessOutboxMessageCommand Command { get; set; }
        public TestableMessageHandlerContext Context { get; set; }
        public ProcessOutboxMessageCommandHandler Handler { get; set; }
        public OutboxMessage OutboxMessage { get; set; }
        public string EndpointName { get; set; }
        public List<Event> Events { get; set; }
        
        public ProcessOutboxMessageCommandHandlerTestsFixture()
        {
            Now = DateTime.UtcNow;

            EndpointName = "SFA.DAS.NServiceBus";

            Events = new List<Event>
            {
                new FooEvent { Created = Now.AddDays(-1) },
                new BarEvent { Created = Now }
            };

            OutboxMessage = new OutboxMessage(GuidComb.NewGuidComb(), EndpointName, Events);

            Context = new TestableMessageHandlerContext
            {
                MessageId = OutboxMessage.MessageId.ToString()
            };

            Command = new ProcessOutboxMessageCommand();
            Outbox = new Mock<IOutbox>();

            Outbox.Setup(o => o.GetAsync(OutboxMessage.MessageId)).ReturnsAsync(OutboxMessage);

            Handler = new ProcessOutboxMessageCommandHandler(Outbox.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(Command, Context);
        }
    }
}