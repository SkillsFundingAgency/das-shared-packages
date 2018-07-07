using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;

namespace SFA.DAS.NServiceBus.UnitTests
{
    [TestFixture]
    public class ProcessOutboxMessageCommandHandlerTests : FluentTest<ProcessOutboxMessageCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingProcessOutboxMessageCommand_ThenShouldPublishOutboxMessageEvents()
        {
            return RunAsync(f => f.Handle(), f => f.Context.PublishedMessages.Select(m => m.Message).Cast<Event>().Should().HaveCount(2).And.Match<IEnumerable<Event>>(e =>
                e.ElementAt(0) is FooEvent && e.ElementAt(0).Created == f.Events[0].Created &&
                e.ElementAt(1) is BarEvent && e.ElementAt(1).Created == f.Events[1].Created));
        }
    }

    public class ProcessOutboxMessageCommandHandlerTestsFixture : FluentTestFixture
    {
        public DateTime Now { get; set; }
        public TestableMessageHandlerContext Context { get; set; }
        public List<Event> Events { get; set; }
        public ProcessOutboxMessageCommandHandler Handler { get; set; }
        public ProcessOutboxMessageCommand Command { get; set; }
        public OutboxMessage OutboxMessage { get; set; }
        public Mock<IOutbox> Outbox { get; set; }


        public ProcessOutboxMessageCommandHandlerTestsFixture()
        {
            Now = DateTime.UtcNow;

            Events = new List<Event>
            {
                new FooEvent { Created = Now.AddDays(-1) },
                new BarEvent { Created = Now }
            };

            OutboxMessage = new OutboxMessageBuilder().WithId(GuidComb.NewGuidComb()).WithEvents(Events).Build();

            Context = new TestableMessageHandlerContext
            {
                MessageId = OutboxMessage.Id.ToString()
            };

            Command = new ProcessOutboxMessageCommand();
            Outbox = new Mock<IOutbox>();

            Outbox.Setup(o => o.GetById(OutboxMessage.Id)).ReturnsAsync(OutboxMessage);

            Handler = new ProcessOutboxMessageCommandHandler(Outbox.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(Command, Context);
        }
    }
}