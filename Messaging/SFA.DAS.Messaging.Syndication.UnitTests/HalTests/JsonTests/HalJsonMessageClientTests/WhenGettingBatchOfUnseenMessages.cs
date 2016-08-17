using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Messaging.Syndication.Hal;
using SFA.DAS.Messaging.Syndication.Hal.Json;
using SFA.DAS.Messaging.Syndication.Http;

namespace SFA.DAS.Messaging.Syndication.UnitTests.HalTests.JsonTests.HalJsonMessageClientTests
{
    public class WhenGettingBatchOfUnseenMessages
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
                .Returns(Task.FromResult(string.Empty));

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
        public async Task ThenItShouldReturnAnEmptyListIfNotDataOnFeed()
        {
            // Arrange
            var blankPage = JsonConvert.SerializeObject(new HalPage<TestMessage>
            {
                Links = new HalPageLinks
                {
                },
                Count = 0,
                Embedded = new HalContent<TestMessage>
                {
                    Messages = new TestMessage[0]
                }
            });
            _httpClientWrapper.Setup(c => c.Get(It.IsAny<string>(), It.IsAny<IDictionary<string, string[]>>()))
                .Returns(Task.FromResult(blankPage));

            // Act
            var actual = await _messageClient.GetBatchOfUnseenMessages<TestMessage>(10);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsEmpty(actual);
        }

        [Test]
        public async Task ThenItShouldReturnAllMessagesIfBatchSizeIsBiggerThanAvailable()
        {
            // Arrange
            var page = JsonConvert.SerializeObject(new HalPage<TestMessage>
            {
                Links = new HalPageLinks
                {
                    First = "/first"
                },
                Count = 2,
                Embedded = new HalContent<TestMessage>
                {
                    Messages = new[] { _message1, _message2 }
                }
            });
            _httpClientWrapper.Setup(c => c.Get(It.IsAny<string>(), It.IsAny<IDictionary<string, string[]>>()))
                .Returns(Task.FromResult(page));

            // Act
            var actual = await _messageClient.GetBatchOfUnseenMessages<TestMessage>(10);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public async Task ThenItShouldStartBatchAtFirstMessageIfNoLastSeenPointer()
        {
            // Act
            var actual = await _messageClient.GetBatchOfUnseenMessages<TestMessage>(10);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(_message1.Id, actual.First().Identifier);
        }

        [TestCase("MSG002", "MSG003")]
        [TestCase("MSG003", "MSG004")]
        public async Task ThenItShouldStartFromMessageAfterLastSeenIfThereIsLastSeenPointer(string lastSeenPointer, string expectedFirstMessageId)
        {
            // Arrange
            _feedPositionRepository.Setup(r => r.GetLastSeenMessageIdentifierAsync())
                .Returns(Task.FromResult(lastSeenPointer));

            // Act
            var actual = await _messageClient.GetBatchOfUnseenMessages<TestMessage>(10);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedFirstMessageId, actual.First().Identifier);
        }

        [Test]
        public async Task ThenItShouldReturnAnEmptyListWhenAllMessagesHaveBeenSeen()
        {
            // Arrange
            _feedPositionRepository.Setup(r => r.GetLastSeenMessageIdentifierAsync())
                .Returns(Task.FromResult(_message6.Id));

            // Act
            var actual = await _messageClient.GetBatchOfUnseenMessages<TestMessage>(10);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsEmpty(actual);
        }

        [Test]
        public async Task ThenItShouldReturnMessagesAcrossFeedPages()
        {

            // Arrange
            _feedPositionRepository.Setup(r => r.GetLastSeenMessageIdentifierAsync())
                .Returns(Task.FromResult(_message2.Id));

            // Act
            var actual = (await _messageClient.GetBatchOfUnseenMessages<TestMessage>(10)).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(_message3.Id, actual[0].Identifier);
            Assert.AreEqual(_message4.Id, actual[1].Identifier);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectNumberOfItemsIfBatchSizeIsLessThanAvailable()
        {
            // Act
            var actual = (await _messageClient.GetBatchOfUnseenMessages<TestMessage>(2)).ToArray();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(_message1.Id, actual[0].Identifier);
            Assert.AreEqual(_message2.Id, actual[1].Identifier);
        }

    }
}
