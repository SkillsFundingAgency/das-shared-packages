using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SFA.DAS.NServiceBus.UnitTests
{
    [TestFixture]
    public class OutboxMessageTests : FluentTest<OutboxMessageTestsFixture>
    {
        [Test]
        public void New_WhenCreatingAnOutboxMessage_ThenShouldCreateOutboxMessage()
        {
            Run(f => f.New(), (f, r) => r.Should().NotBeNull().And.Match<OutboxMessage>(m =>
                m.Id != Guid.Empty &&
                m.Sent >= f.Now &&
                m.Published == null &&
                m.Data == JsonConvert.SerializeObject(f.Events, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })));
        }

        [Test]
        public void Publish_WhenPublishing_ThenShouldPublishOutboxMessage()
        {
            Run(f => f.Publish(), f => f.OutboxMessage.Published.Should().BeOnOrAfter(f.Now));
        }

        [Test]
        public void Publish_WhenPublishing_ThenShouldReturnEvents()
        {
            Run(f => f.Publish(), (f, r) => r.Should().HaveCount(2).And.Match<IEnumerable<Event>>(e =>
                e.ElementAt(0) is FooEvent && e.ElementAt(0).Created == f.Events[0].Created &&
                e.ElementAt(1) is BarEvent && e.ElementAt(1).Created == f.Events[1].Created));
        }

        [Test]
        public void Publish_WhenRepublishing_ThenShouldThrowException()
        {
            Run(f => f.Republish(), (f, a) => a.ShouldThrow<Exception>().WithMessage("Requires not already published"));
        }
    }

    public class OutboxMessageTestsFixture : FluentTestFixture
    {
        public DateTime Now { get; set; }
        public List<Event> Events { get; set; }
        public OutboxMessage OutboxMessage { get; set; }

        public OutboxMessageTestsFixture()
        {
            Now = DateTime.UtcNow;

            Events = new List<Event>
            {
                new FooEvent { Created = Now.AddDays(-1) },
                new BarEvent { Created = Now }
            };
        }

        public OutboxMessage New()
        {
            return OutboxMessage = new OutboxMessage(Events);
        }

        public IEnumerable<Event> Publish()
        {
            var outboxMessage = New();

            return outboxMessage.Publish();
        }

        public IEnumerable<Event> Republish()
        {
            var outboxMessage = New();

            outboxMessage.Publish();

            return outboxMessage.Publish();
        }
    }
}