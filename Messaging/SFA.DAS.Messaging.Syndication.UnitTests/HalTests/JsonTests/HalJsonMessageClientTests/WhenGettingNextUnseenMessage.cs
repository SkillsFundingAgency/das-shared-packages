using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Messaging.Syndication.Hal;
using SFA.DAS.Messaging.Syndication.Hal.Json;
using SFA.DAS.Messaging.Syndication.Http;

namespace SFA.DAS.Messaging.Syndication.UnitTests.HalTests.JsonTests.HalJsonMessageClientTests
{
    public class WhenGettingNextUnseenMessage
    {
        private readonly TestMessage _message1 = new TestMessage { Id = "MSG001" };
        private readonly TestMessage _message2 = new TestMessage { Id = "MSG002" };
        private readonly TestMessage _message3 = new TestMessage { Id = "MSG003" };
        private readonly TestMessage _message4 = new TestMessage { Id = "MSG004" };
        private readonly TestMessage _message5 = new TestMessage { Id = "MSG005" };
        private readonly TestMessage _message6 = new TestMessage { Id = "MSG006" };

        private Mock<IFeedPositionRepository> _feedPositionRepository;
        private Mock<IHttpClientWrapper> _httpClientWrapper;
        private Mock<IMessageIdentifier<TestMessage>> _messageIdentifier;
        private Mock<IMessageIdentifierFactory> _messageIdentifierFactory;
        private HalJsonMessageClient _messageClient;

        [SetUp]
        public void Arrange()
        {
            _feedPositionRepository = new Mock<IFeedPositionRepository>();
            _feedPositionRepository.Setup(r => r.GetLastSeenMessageIdentifierAsync())
                .Returns(Task.FromResult("MSG002"));

            var page1 = JsonConvert.SerializeObject(new HalPage<TestMessage>
            {
                Links = new HalPageLinks
                {
                    Next = "/page2",
                    First = "/",
                    Last = "/page2"
                },
                Count = 6,
                Embedded = new HalContent<TestMessage>
                {
                    Messages = new[] { _message1, _message2, _message3 }
                }
            });
            var page2 = JsonConvert.SerializeObject(new HalPage<TestMessage>
            {
                Links = new HalPageLinks
                {
                    Prev = "/",
                    First = "/",
                    Last = "/page2"
                },
                Count = 6,
                Embedded = new HalContent<TestMessage>
                {
                    Messages = new[] { _message4, _message5, _message6 }
                }
            });
            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _httpClientWrapper.Setup(c => c.Get("/", It.IsAny<IDictionary<string, string[]>>()))
                .Returns(Task.FromResult(page1));
            _httpClientWrapper.Setup(c => c.Get("/page2", It.IsAny<IDictionary<string, string[]>>()))
                .Returns(Task.FromResult(page2));

            _messageIdentifier = new Mock<IMessageIdentifier<TestMessage>>();
            _messageIdentifier.Setup(x => x.GetIdentifier(It.IsAny<TestMessage>()))
                .Returns((TestMessage msg) => msg.Id);
            _messageIdentifierFactory = new Mock<IMessageIdentifierFactory>();
            _messageIdentifierFactory.Setup(f => f.Create<TestMessage>())
                .Returns(_messageIdentifier.Object);

            _messageClient = new HalJsonMessageClient(_feedPositionRepository.Object, _httpClientWrapper.Object, _messageIdentifierFactory.Object);
        }

        [Test]
        public async Task ThenItShouldReturnNullIfNoMessages()
        {
            // Arrange
            _httpClientWrapper.Setup(c => c.Get("/", It.IsAny<IDictionary<string, string[]>>()))
                .Returns(Task.FromResult<string>(null));

            // Act
            var actual = await _messageClient.GetNextUnseenMessage<TestMessage>();

            // Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task ThenItShouldReturnOldestMessageIfNotSeenAnyMessagesYet()
        {
            // Arrange
            _feedPositionRepository.Setup(r => r.GetLastSeenMessageIdentifierAsync())
                .Returns(Task.FromResult<string>(null));

            // Act
            var actual = await _messageClient.GetNextUnseenMessage<TestMessage>();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(_message1.Id, actual.Identifier);
        }

        [TestCase("MSG001", "MSG002")]
        [TestCase("MSG002", "MSG003")]
        [TestCase("MSG003", "MSG004")]
        [TestCase("MSG004", "MSG005")]
        [TestCase("MSG005", "MSG006")]
        public async Task ThenItShouldReturnTheNextMostRecentOneSinceLastSeenMessage(string lastSeenMessageId, string expectedNextMessageId)
        {
            // Arrange
            _feedPositionRepository.Setup(r => r.GetLastSeenMessageIdentifierAsync())
                .Returns(Task.FromResult(lastSeenMessageId));

            // Act
            var actual = await _messageClient.GetNextUnseenMessage<TestMessage>();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedNextMessageId, actual.Identifier);
        }

        [Test]
        public async Task ThenItShouldReturnNullIfUpToDateWithMessages()
        {
            // Arrange
            _feedPositionRepository.Setup(r => r.GetLastSeenMessageIdentifierAsync())
                .Returns(Task.FromResult(_message6.Id));

            // Act
            var actual = await _messageClient.GetNextUnseenMessage<TestMessage>();

            // Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task ThenItShouldReturnOldestMessageIdLastSeenIdIsNotInFeed()
        {

            // Arrange
            _feedPositionRepository.Setup(r => r.GetLastSeenMessageIdentifierAsync())
                .Returns(Task.FromResult("XXX"));

            // Act
            var actual = await _messageClient.GetNextUnseenMessage<TestMessage>();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(_message1.Id, actual.Identifier);
        }
    }
}
