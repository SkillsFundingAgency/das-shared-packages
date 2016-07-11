using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SFA.DAS.Messaging.Syndication.Hal;
using SFA.DAS.Messaging.Syndication.Hal.Json;

namespace SFA.DAS.Messaging.Syndication.UnitTests.HalTests.JsonTests.HalJsonMessageServiceTests
{
    public class WhenGettingAPage
    {
        private Mock<IMessageRepository> _messageRepository;
        private Mock<IHalResourceAttributeExtrator<TestMessage>> _halResourceAttributeExtrator;
        private HalJsonMessageService<TestMessage> _messageService;
        private Mock<IHalPageLinkBuilder> _halPageLinkBuilder;

        [SetUp]
        public void Arrange()
        {
            _messageRepository = new Mock<IMessageRepository>();
            _messageRepository.Setup(r => r.RetreivePageAsync<TestMessage>(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new SyndicationPage<TestMessage>
                {
                    Messages = new[]
                    {
                        new TestMessage()
                    },
                    PageNumber = 2,
                    TotalNumberOfMessages = 100
                }));

            _halResourceAttributeExtrator = new Mock<IHalResourceAttributeExtrator<TestMessage>>();
            _halResourceAttributeExtrator.Setup(x => x.Extract(It.IsAny<TestMessage>()))
                .Returns(new HalResourceAttributes
                {
                    Links = new Dictionary<string, string>
                    {
                        { "self", "me" }
                    },
                    Properties = new Dictionary<string, string>
                    {
                        { "Id", "some-id" }
                    }
                });

            _halPageLinkBuilder = new Mock<IHalPageLinkBuilder>();
            _halPageLinkBuilder.Setup(x => x.NextPage(It.IsAny<int>())).Returns("/nextpage");
            _halPageLinkBuilder.Setup(x => x.PreviousPage(It.IsAny<int>())).Returns("/prevpage");
            _halPageLinkBuilder.Setup(x => x.FirstPage(It.IsAny<int>())).Returns("/first");
            _halPageLinkBuilder.Setup(x => x.LastPage(It.IsAny<int>())).Returns("/last");

            _messageService = new HalJsonMessageService<TestMessage>(_messageRepository.Object, _halResourceAttributeExtrator.Object, _halPageLinkBuilder.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAPageOfResults()
        {
            // Act
            var actual = await _messageService.GetPageAsync(2, 10);

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldReturnHalJsonContentTypeHeader()
        {
            // Act
            var actual = await _messageService.GetPageAsync(2, 10);

            // Assert
            Assert.IsNotNull(actual.Headers);
            Assert.IsTrue(actual.Headers.ContainsKey("Content-Type"));
            Assert.AreEqual("application/hal+json", actual.Headers["Content-Type"][0]);
        }

        [Test]
        public async Task ThenItShouldReturnJsonSerialisedPageOfResults()
        {
            // Act
            var actual = await _messageService.GetPageAsync(2, 10);

            // Assert
            Assert.IsNotNull(actual.Content);

            var jsonContent = JObject.Parse(actual.Content);
            Assert.IsNotNull(jsonContent);
            Assert.IsNotNull(jsonContent["_links"]);
            Assert.IsNotNull(jsonContent["_embedded"]);
        }

        [Test]
        public async Task ThenItShouldIncludeEmbeddedMessage()
        {
            // Act
            var actual = await _messageService.GetPageAsync(2, 10);

            // Assert
            Assert.IsNotNull(actual.Content);

            var jsonContent = JObject.Parse(actual.Content);
            var embeddedMessages = jsonContent["_embedded"]["messages"] as JArray;
            Assert.IsNotNull(embeddedMessages);
            Assert.AreEqual(1, embeddedMessages.Count);
            Assert.AreEqual("some-id", embeddedMessages[0]["id"].Value<string>());
            Assert.AreEqual("me", embeddedMessages[0]["_links"]["self"].Value<string>());
        }

        [TestCase(2, "/nextpage", "/prevpage", "/first", "/last")]
        [TestCase(1, "/nextpage", null, "/first", "/last")]
        [TestCase(10, null, "/prevpage", "/first", "/last")]
        public async Task ThenItShouldIncludePageLinks(int page, string expectedNext, string expectedPrev, string expectedFirst, string expectedLast)
        {
            // Act
            var actual = await _messageService.GetPageAsync(page, 10);

            // Assert
            Assert.IsNotNull(actual.Content);

            var jsonContent = JObject.Parse(actual.Content);
            Assert.IsNotNull(jsonContent["_links"]);
            AssertJsonLinkCorrect(jsonContent, "next", expectedNext);
            AssertJsonLinkCorrect(jsonContent, "prev", expectedPrev);
            AssertJsonLinkCorrect(jsonContent, "first", expectedFirst);
            AssertJsonLinkCorrect(jsonContent, "last", expectedLast);
        }

        [Test]
        public async Task ThenItShouldNotIncludeFirstAndLastLinksIfNoRecords()
        {
            // Arrange
            _messageRepository.Setup(r => r.RetreivePageAsync<TestMessage>(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new SyndicationPage<TestMessage>
                {
                    Messages = new TestMessage[0],
                    PageNumber = 1,
                    TotalNumberOfMessages = 0
                }));

            // Act
            var actual = await _messageService.GetPageAsync(1, 10);

            // Assert
            Assert.IsNotNull(actual.Content);

            var jsonContent = JObject.Parse(actual.Content);
            Assert.IsNotNull(jsonContent["_links"]);
            AssertJsonLinkCorrect(jsonContent, "first", null);
            AssertJsonLinkCorrect(jsonContent, "last", null);
        }



        private void AssertJsonLinkCorrect(JObject jsonContainer, string linkName, string expectedUrl)
        {
            if (expectedUrl == null)
            {
                Assert.IsNull(jsonContainer["_links"][linkName]);
            }
            else
            {
                Assert.AreEqual(expectedUrl, jsonContainer["_links"][linkName].Value<string>());
            }
        }
    }
}
