using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests
{
    [TestFixture]
    public class ProcessOutboxMessagesJobTests : FluentTest<ProcessOutboxMessagesJobTestsFixture>
    {
        [Test]
        public Task RunAsync_WhenRunningAndOutboxMessagesAreAwaitingDispatch_ThenShouldSendProcessOutboxMessageCommands()
        {
            return RunAsync(f => f.SetOutboxMessagesAwaitingDispatch(), f => f.RunAsync(), f =>
            {
                f.Session.SentMessages.Select(m => m.Message).Should().HaveCount(2).And.ContainItemsAssignableTo<ProcessOutboxMessageCommand>();
                f.Session.SentMessages.Select(m => new { MessageId = Guid.Parse(m.Options.GetMessageId()), EndpointName = m.Options.GetDestination() }).ShouldAllBeEquivalentTo(f.OutboxMessages);
            });
        }

        [Test]
        public Task RunAsync_WhenRunningAndNoOutboxMessagesAreAwaitingDispatch_ThenShouldNotSendProcessOutboxMessageCommands()
        {
            return RunAsync(f => f.RunAsync(), f => f.Session.SentMessages.Should().BeEmpty());
        }
    }

    public class ProcessOutboxMessagesJobTestsFixture : FluentTestFixture
    {
        public TestableMessageSession Session { get; set; }
        public List<IOutboxMessageAwaitingDispatch> OutboxMessages { get; set; }
        public IProcessOutboxMessagesJob Job { get; set; }
        public Mock<IOutbox> Outbox { get; set; }

        public ProcessOutboxMessagesJobTestsFixture()
        {
            Session = new TestableMessageSession();
            OutboxMessages = new List<IOutboxMessageAwaitingDispatch>();
            Outbox = new Mock<IOutbox>();

            Outbox.Setup(o => o.GetAwaitingDispatchAsync()).ReturnsAsync(OutboxMessages);

            Job = new ProcessOutboxMessagesJob(Session, Outbox.Object);
        }

        public Task RunAsync()
        {
            return Job.RunAsync();
        }

        public ProcessOutboxMessagesJobTestsFixture SetOutboxMessagesAwaitingDispatch()
        {
            OutboxMessages.Add(new OutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Foo"));
            OutboxMessages.Add(new OutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Bar"));

            return this;
        }
    }
}