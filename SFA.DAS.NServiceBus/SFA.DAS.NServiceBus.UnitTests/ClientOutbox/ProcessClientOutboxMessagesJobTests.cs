using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.NServiceBus.ClientOutbox.Commands;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.ClientOutbox
{
    [TestFixture]
    public class ProcessClientOutboxMessagesJobTests : FluentTest<ProcessOutboxMessagesJobTestsFixture>
    {
        [Test]
        public Task RunAsync_WhenRunningAndClientOutboxMessagesAreAwaitingDispatch_ThenShouldSendProcessClientOutboxMessageCommands()
        {
            return RunAsync(f => f.SetClientOutboxMessagesAwaitingDispatch(), f => f.RunAsync(), f =>
            {
                f.MessageSession.SentMessages.Select(m => m.Message).Should().HaveCount(2).And.ContainItemsAssignableTo<ProcessClientOutboxMessageCommand>();
                f.MessageSession.SentMessages.Select(m => new { MessageId = Guid.Parse(m.Options.GetMessageId()), EndpointName = m.Options.GetDestination() }).Should().BeEquivalentTo(f.ClientOutboxMessages);
            });
        }

        [Test]
        public Task RunAsync_WhenRunningAndNoClientOutboxMessagesAreAwaitingDispatch_ThenShouldNotSendProcessClientOutboxMessageCommands()
        {
            return RunAsync(f => f.RunAsync(), f => f.MessageSession.SentMessages.Should().BeEmpty());
        }
    }

    public class ProcessOutboxMessagesJobTestsFixture
    {
        public TestableMessageSession MessageSession { get; set; }
        public List<IClientOutboxMessageAwaitingDispatch> ClientOutboxMessages { get; set; }
        public IProcessClientOutboxMessagesJob Job { get; set; }
        public Mock<IClientOutboxStorage> ClientOutboxStorage { get; set; }

        public ProcessOutboxMessagesJobTestsFixture()
        {
            MessageSession = new TestableMessageSession();
            ClientOutboxMessages = new List<IClientOutboxMessageAwaitingDispatch>();
            ClientOutboxStorage = new Mock<IClientOutboxStorage>();

            ClientOutboxStorage.Setup(o => o.GetAwaitingDispatchAsync()).ReturnsAsync(ClientOutboxMessages);

            Job = new ProcessClientOutboxMessagesJob(MessageSession, ClientOutboxStorage.Object);
        }

        public Task RunAsync()
        {
            return Job.RunAsync();
        }

        public ProcessOutboxMessagesJobTestsFixture SetClientOutboxMessagesAwaitingDispatch()
        {
            ClientOutboxMessages.Add(new ClientOutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Foo"));
            ClientOutboxMessages.Add(new ClientOutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Bar"));

            return this;
        }
    }
}