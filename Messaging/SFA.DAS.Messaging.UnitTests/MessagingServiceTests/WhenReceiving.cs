using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SFA.DAS.Messaging.UnitTests.MessagingServiceTests
{
    public class WhenReceiving
    {
        private Mock<IMessageSubSystem> _messageSubSystem;
        private MessagingService _messagingService;

        [SetUp]
        public void Arrange()
        {
            _messageSubSystem = new Mock<IMessageSubSystem>();

            _messagingService = new MessagingService(_messageSubSystem.Object);
        }

        [Test]
        public async Task ThenItShouldReturnADeserializedEvent()
        {
            //Arrange
            var expectedEvent = TestEvent.GetDefault();
            var expectedMessage = MockMessageFromContent(JsonConvert.SerializeObject(expectedEvent));
            _messageSubSystem.Setup(x => x.Dequeue()).Returns(Task.FromResult(expectedMessage.Object));

            //Act
            var actual = await _messagingService.Receive<TestEvent>();
            
            //Assert
            Assert.AreEqual(expectedEvent.Timestamp, actual.Content.Timestamp);
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task ThenItShouldReturnNullContentWhenNoMessageIsDequeued(string expectedEvent)
        {
            //Arrange
            var expectedMessage = MockMessageFromContent(JsonConvert.SerializeObject(expectedEvent));
            _messageSubSystem.Setup(x => x.Dequeue()).Returns(Task.FromResult(expectedMessage.Object));

            //Act
            var actual = await _messagingService.Receive<TestEvent>();

            //Assert
            Assert.IsNull(actual.Content);
        }


        private Mock<SubSystemMessage> MockMessageFromContent(string content)
        {
            var mock = new Mock<SubSystemMessage>();
            mock.Setup(m => m.Content).Returns(content);
            return mock;
        }
    }
}
