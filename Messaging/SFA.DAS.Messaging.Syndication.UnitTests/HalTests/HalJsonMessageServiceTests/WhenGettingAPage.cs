using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SFA.DAS.Messaging.Syndication.Hal;

namespace SFA.DAS.Messaging.Syndication.UnitTests.HalTests.HalJsonMessageServiceTests
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

        [TestCase(2, "/nextpage", "/prevpage")]
        [TestCase(1, "/nextpage", null)]
        [TestCase(10, null, "/prevpage")]
        public async Task ThenItShouldIncludePageLinks(int page, string expectedNext, string expectedPrev)
        {
            // Act
            var actual = await _messageService.GetPageAsync(page, 10);

            // Assert
            Assert.IsNotNull(actual.Content);

            var jsonContent = JObject.Parse(actual.Content);
            Assert.IsNotNull(jsonContent["_links"]);

            if (expectedNext == null)
            {
                Assert.IsNull(jsonContent["_links"]["next"]);
            }
            else
            {
                Assert.AreEqual(expectedNext, jsonContent["_links"]["next"].Value<string>());
            }
            if (expectedPrev == null)
            {
                Assert.IsNull(jsonContent["_links"]["prev"]);
            }
            else
            {
                Assert.AreEqual(expectedPrev, jsonContent["_links"]["prev"].Value<string>());
            }
        }
    }
}
