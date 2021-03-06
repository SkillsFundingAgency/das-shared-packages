﻿using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork.Context;
using SFA.DAS.UnitOfWork.NServiceBus.Services;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests.Services
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

    public class EventPublisherTestsFixture
    {
        public FooEvent FooEvent { get; set; }
        public Func<BarEvent> BarEvent { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public IEventPublisher EventPublisher { get; set; }

        public EventPublisherTestsFixture()
        {
            FooEvent = new FooEvent(DateTime.UtcNow);
            BarEvent = () => new BarEvent(DateTime.Now);
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            EventPublisher = new EventPublisher(UnitOfWorkContext.Object);
        }

        public Task Publish()
        {
            return Task.WhenAll(EventPublisher.Publish(FooEvent), EventPublisher.Publish(BarEvent));
        }
    }
}