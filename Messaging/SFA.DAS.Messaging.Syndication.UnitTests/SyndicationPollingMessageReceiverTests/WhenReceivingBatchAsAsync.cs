using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Messaging.Syndication.UnitTests.SyndicationPollingMessageReceiverTests
{
    public class WhenReceivingBatchAsAsync
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
        public async Task ThenItShouldReturnEmptyEnumerableWhenNoMessages()
        {
            // Act
            var actual = await _receiver.ReceiveBatchAsAsync<TestMessage>(10);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsEmpty(actual);
        }

        [Test]
        public async Task ThenItShouldReturnMessagesFromClient()
        {
            // Arrange
            var expectedContent1 = new TestMessage();
            var expectedContent2 = new TestMessage();
            _messageClient.Setup(c => c.GetBatchOfUnseenMessages<TestMessage>(10))
                .Returns(Task.FromResult<IEnumerable<ClientMessage<TestMessage>>>(new[]
                {
                    new ClientMessage<TestMessage> { Identifier = "1", Message = expectedContent1 },
                    new ClientMessage<TestMessage> { Identifier = "2", Message = expectedContent2 }
                }));

            // Act
            var actual = (await _receiver.ReceiveBatchAsAsync<TestMessage>(10)).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreSame(expectedContent1, actual[0].Content);
            Assert.AreSame(expectedContent2, actual[1].Content);
        }
    }
}
