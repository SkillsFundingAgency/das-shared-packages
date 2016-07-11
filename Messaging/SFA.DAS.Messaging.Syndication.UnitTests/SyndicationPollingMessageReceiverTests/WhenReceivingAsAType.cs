using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Messaging.Syndication.UnitTests.SyndicationPollingMessageReceiverTests
{
    public class WhenReceivingAsAType
    {
        private Mock<IMessageClient> _messageClient;
        private Mock<IFeedPositionRepository> _feedPositionRepository;
        private SyndicationPollingMessageReceiver _receiver;

        [SetUp]
        public void Arrange()
        {
            _messageClient = new Mock<IMessageClient>();

            _feedPositionRepository = new Mock<IFeedPositionRepository>();

            _receiver = new SyndicationPollingMessageReceiver(_messageClient.Object, _feedPositionRepository.Object);
        }

        [Test]
        public async Task ThenItShouldReturnNullWhenNoMessages()
        {
            // Act
            var actual = await _receiver.ReceiveAsAsync<TestMessage>();

            // Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task ThenItShouldReturnMessageFromClient()
        {
            // Arrange
            var expectedContent = new TestMessage();
            _messageClient.Setup(c => c.GetNextUnseenMessage<TestMessage>())
                .Returns(Task.FromResult(new ClientMessage<TestMessage> { Identifier = "MSG123", Message = expectedContent }));

            // Act
            var actual = await _receiver.ReceiveAsAsync<TestMessage>();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreSame(expectedContent, actual.Content);
        }

    }
}
