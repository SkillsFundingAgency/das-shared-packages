using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests
{
    [TestFixture]
    public class EventPublisherTests : FluentTest<EventPublisherTestsFixture>
    {
        [Test]
        public Task Publish_WhenPublishingEvents_ThenShouldAddEventsToUnitOfWorkContext()
        {
            return RunAsync(f => f.Publish(), f =>
            {
                f.UnitOfWorkContext.Verify(c => c.AddEvent(f.FooEvent));
                f.UnitOfWorkContext.Verify(c => c.AddEvent(f.BarEvent));
            });
        }
    }

    public class EventPublisherTestsFixture : FluentTestFixture
    {
        public FooEvent FooEvent { get; set; }
        public Action<BarEvent> BarEvent { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public IEventPublisher EventPublisher { get; set; }

        public EventPublisherTestsFixture()
        {
            FooEvent = new FooEvent { Created = DateTime.UtcNow };
            BarEvent = e => e.Created = DateTime.Now;
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            EventPublisher = new EventPublisher(UnitOfWorkContext.Object);
        }

        public Task Publish()
        {
            return Task.WhenAll(EventPublisher.Publish(FooEvent), EventPublisher.Publish(BarEvent));
        }
    }
}