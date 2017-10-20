using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Messaging.Syndication.UnitTests.SyndicationMessagePublisherTests
{
    public class WhenPublishing
    {
        private Mock<IMessageRepository> _messageRepository;
        private SyndicationMessagePublisher _publisher;

        [SetUp]
        public void Arrange()
        {
            _messageRepository = new Mock<IMessageRepository>();

            _publisher = new SyndicationMessagePublisher(_messageRepository.Object);
        }

        [Test]
        public async Task ThenItShouldStoreMessage()
        {
            // Arrange
            var message = new TestMessage();

            // Act
            await _publisher.PublishAsync(message);

            // Assert
            _messageRepository.Verify(r => r.StoreAsync(message), Times.Once());
        }
    }
}
