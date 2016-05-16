using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SFA.DAS.Messaging.UnitTests.MessagingServiceTests
{
    public class WhenPublishing
    {
        private TestEvent _basicEvent;
        private string _basicEventJson;
        private Mock<IMessageSubSystem> _messageSubSystem;
        private MessagingService _messageService;

        [SetUp]
        public void Arrange()
        {
            _basicEvent = TestEvent.GetDefault();
            _basicEventJson = JsonConvert.SerializeObject(_basicEvent);

            _messageSubSystem = new Mock<IMessageSubSystem>();
            _messageSubSystem.Setup(ss => ss.Enqueue(It.IsAny<string>())).Returns(Task.FromResult<object>(null));

            _messageService = new MessagingService(_messageSubSystem.Object);
        }

        [Test]
        public async Task ThenItShouldEnqueueJsonSerializedMessage()
        {
            // Act
            await _messageService.Publish(_basicEvent);

            // Assert
            _messageSubSystem.Verify(ss => ss.Enqueue(_basicEventJson), Times.Once());
        }

        [Test]
        public async Task ThenItShouldNotQueueWhenTheMessageIsNull()
        {
            // Act
            await _messageService.Publish<TestEvent>(null);

            // Assert
            _messageSubSystem.Verify(ss => ss.Enqueue(It.IsAny<string>()), Times.Never());
        }
    }
}
