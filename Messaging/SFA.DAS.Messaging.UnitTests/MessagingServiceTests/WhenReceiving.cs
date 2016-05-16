using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            _messageSubSystem.Setup(x => x.Dequeue()).Returns(Task.FromResult(JsonConvert.SerializeObject(expectedEvent)));

            //Act
            var actual = await _messagingService.Receive<TestEvent>();
            
            //Assert
            Assert.AreEqual(expectedEvent.Timestamp, actual.Timestamp);
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task ThenItShouldReturnNullWhenNoMessageIsDequeued(string expectedEvent)
        {
            //Arrange
            _messageSubSystem.Setup(x => x.Dequeue()).Returns(Task.FromResult(expectedEvent));

            //Act
            var actual = await _messagingService.Receive<TestEvent>();

            //Assert
            Assert.IsNull(actual);
        }
    }
}
