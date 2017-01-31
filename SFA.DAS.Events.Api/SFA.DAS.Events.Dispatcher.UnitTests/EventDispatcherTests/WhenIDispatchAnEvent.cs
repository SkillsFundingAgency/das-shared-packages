using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Events.Dispatcher.UnitTests.EventDispatcherTests
{
    [TestFixture]
    public class WhenIDispatchAnEvent
    {
        private EventDispatcher _dispatcher;

        [SetUp]
        public void Arrange()
        {
            _dispatcher = new EventDispatcher();
        }

        [Test]
        public async Task ThenTheCorrectHandlerIsCalled()
        {
            var correctHandler = new Mock<IEventHandler<ExpectedTestEvent>>();
            var otherEventName = new Mock<IEventHandler<ExpectedTestEvent>>();
            var otherEventType = new Mock<IEventHandler<OtherTestEvent>>();

            await _dispatcher.RegisterHandler(correctHandler.Object, "CorrectEvent");
            await _dispatcher.RegisterHandler(otherEventName.Object, "IncorrectEvent");
            await _dispatcher.RegisterHandler(otherEventType.Object, "CorrectEvent");

            var @event = new ExpectedTestEvent { Event = "CorrectEvent", Id = 1235 };

            await _dispatcher.Dispatch(@event);

            //correctHandler.Verify(x => x.Handle(@event), Times.Once);
            otherEventName.Verify(x => x.Handle(It.IsAny<ExpectedTestEvent>()), Times.Never);
            otherEventType.Verify(x => x.Handle(It.IsAny<OtherTestEvent>()), Times.Never);
        }

        [Test]
        public async Task AndNoHandlerIsRegisteredThenAnExceptionIsThrown()
        {
            var correctHandler = new Mock<IEventHandler<ExpectedTestEvent>>();
            var otherEventType = new Mock<IEventHandler<OtherTestEvent>>();

            await _dispatcher.RegisterHandler(correctHandler.Object, "CorrectEvent");
            await _dispatcher.RegisterHandler(otherEventType.Object, "CorrectEvent");

            var @event = new ExpectedTestEvent { Event = "IncorrectEvent", Id = 1235 };

            var exception = Assert.ThrowsAsync<HandlerNotRegisteredException>(() => _dispatcher.Dispatch(@event));

            exception.EventName.Should().Be(@event.Event);
            exception.EventType.Should().Be(@event.GetType());

            correctHandler.Verify(x => x.Handle(It.IsAny<ExpectedTestEvent>()), Times.Never);
            otherEventType.Verify(x => x.Handle(It.IsAny<OtherTestEvent>()), Times.Never);
        }

        [Test]
        public async Task AndThereAreMultipleHandlersThenAllOfTheHandlersAreCalled()
        {
            var firstCorrectHandler = new Mock<IEventHandler<ExpectedTestEvent>>();
            var otherEventName = new Mock<IEventHandler<ExpectedTestEvent>>();
            var secondCorrectHandler = new Mock<IEventHandler<ExpectedTestEvent>>();

            await _dispatcher.RegisterHandler(firstCorrectHandler.Object, "CorrectEvent");
            await _dispatcher.RegisterHandler(otherEventName.Object, "IncorrectEvent");
            await _dispatcher.RegisterHandler(secondCorrectHandler.Object, "CorrectEvent");

            var @event = new ExpectedTestEvent { Event = "CorrectEvent", Id = 1235 };

            await _dispatcher.Dispatch(@event);

            firstCorrectHandler.Verify(x => x.Handle(@event), Times.Once);
            otherEventName.Verify(x => x.Handle(It.IsAny<ExpectedTestEvent>()), Times.Never);
            secondCorrectHandler.Verify(x => x.Handle(It.IsAny<ExpectedTestEvent>()), Times.Once);
        }
    }
}
