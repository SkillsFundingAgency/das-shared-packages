using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.NServiceBus;
using SFA.DAS.Testing;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests
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
        public Func<BarEvent> BarEvent { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public IEventPublisher EventPublisher { get; set; }

        public EventPublisherTestsFixture()
        {
            FooEvent = new FooEvent { Created = DateTime.UtcNow };
            BarEvent = () => new BarEvent { Created = DateTime.Now };
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            EventPublisher = new EventPublisher(UnitOfWorkContext.Object);
        }

        public Task Publish()
        {
            return Task.WhenAll(EventPublisher.Publish(FooEvent), EventPublisher.Publish(BarEvent));
        }
    }
}