using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.Messaging.Syndication.UnitTests.SyndicationMessageTests
{
    public class WhenAborting
    {
        private Mock<IFeedPositionRepository> _feedPositionRepository;
        private SyndicationMessage<TestMessage> _message;

        [SetUp]
        public void Arrange()
        {
            _feedPositionRepository = new Mock<IFeedPositionRepository>();
            _feedPositionRepository.Setup(r => r.UpdateLastSeenMessageIdentifierAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<object>(null));

            _message = new SyndicationMessage<TestMessage>(new TestMessage(), "TEST001", _feedPositionRepository.Object);
        }

        [Test]
        public async Task ThenItShouldNotUpdatePositionToMessageId()
        {
            // Act
            await _message.AbortAsync();

            // Assert
            _feedPositionRepository.Verify(r => r.UpdateLastSeenMessageIdentifierAsync("TEST001"), Times.Never);
        }
    }
}
