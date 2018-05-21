using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Messaging.Exceptions;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.UnitTests
{
    [TestFixture]
    public class MessageContextProviderTests
    {
        [Test]
        public void Constructor_Valid_Test()
        {
            var mcp = new MessageContextProvider();
            Assert.Pass("Did not throw exception");
        }

        [Test]
        public void StoreMessageContext_NullMessage_ThrowsException()
        {
            var mcp = new MessageContextProvider();

            Assert.Throws<MessageContextException>(() => mcp.StoreMessageContext((IMessage<Object>)null));
        }

        [Test]
        public void StoreMessageContext_NullMessageContent_ThrowsException()
        {
            var mcp = new MessageContextProvider();
            var message = new Mock<IMessage<object>>();

            Assert.Throws<MessageContextException>(() => mcp.StoreMessageContext(message.Object));
        }

        [Test]
        public void StoreMessageContext_ValidMessageContent_ShouldNotThrowException()
        {
            // Arrange
            var mcp = new MessageContextProvider();
            var message = new Mock<IMessage<object>>();
            message.Setup(mc => mc.Content).Returns(new object());

            // Act
            mcp.StoreMessageContext(message.Object);

            // Assert
            Assert.Pass("Did not get an exception");
        }

        [Test]
        public void GetContextForMessageBody_ValidMessageContent_ShouldBeAvailableAfterStore()
        {
            // Arrange
            var mcp = new MessageContextProvider();
            var message = new Mock<IMessage<object>>();
            var messageContent = new object();
            message.Setup(mc => mc.Content).Returns(messageContent);
            mcp.StoreMessageContext(message.Object);

            // Act
            var messageContext = mcp.GetContextForMessageBody(messageContent);

            // Assert
            Assert.IsNotNull(messageContext);
        }

        [Test]
        public void GetContextForMessageBody_ValidMessageContent_ShouldBeSameInstanceRetrievedWithEachCallForTheSameContent()
        {
            // Arrange
            var mcp = new MessageContextProvider();
            var message = new Mock<IMessage<object>>();
            var messageContent = new object();
            message.Setup(mc => mc.Content).Returns(messageContent);
            mcp.StoreMessageContext(message.Object);

            // Act
            var messageContext1 = mcp.GetContextForMessageBody(messageContent);
            var messageContext2 = mcp.GetContextForMessageBody(messageContent);

            // Assert
            Assert.AreEqual(messageContext1, messageContext2);
        }


        [Test]
        public void GetContextForMessageBody_ValidMessageContent_ShouldBeAbleToHandleMultipleParallelMessages()
        {
            // Arrange
            var mcp = new MessageContextProvider();

            var messages = new List<Mock<IMessage<object>>>();
            for (int i = 0; i < 500; i++)
            {
                var messageMock = new Mock<IMessage<object>>();
                var messageContent = new object();
                messageMock.Setup(mc => mc.Content).Returns(messageContent);
                messages.Add(messageMock);
            }

            Parallel.ForEach(messages, message =>
            {
                mcp.StoreMessageContext(message.Object);

                // Act
                var messageContext1 = mcp.GetContextForMessageBody(message.Object.Content);
                var messageContext2 = mcp.GetContextForMessageBody(message.Object.Content);

                // Assert
                Assert.AreEqual(message.Object.Content, messageContext1.MessageContent);
                Assert.AreEqual(messageContext1, messageContext2);
            });
        }

        [Test]
        public void Release_NullMessage_ShouldIgnoreNulls()
        {
            var mcp = new MessageContextProvider();
            mcp.ReleaseMessageContext((IMessage<object>) null);
            Assert.Pass("Did not get an exception thrown");
        }

        [Test]
        public void Release_NullMessageContent_ShouldIgnoreNulls()
        {
            // Arrange
            var mcp = new MessageContextProvider();
            var message = new Mock<IMessage<object>>();
            message.Setup(mc => mc.Content).Returns(null);

            // Act
            mcp.ReleaseMessageContext(message.Object);
            Assert.Pass("Did not get an exception thrown");
        }

        [Test]
        public void Release_ValidMessageContent_ShouldNotThrowExcptionIfContentIsReleasedOnce()
        {
            // Arrange
            var mcp = new MessageContextProvider();
            var message = new Mock<IMessage<object>>();
            message.Setup(mc => mc.Content).Returns(new object());
            mcp.StoreMessageContext(message.Object);

            // Act
            mcp.ReleaseMessageContext(message.Object);
        }

        [Test]
        public void Release_ValidMessageContent_ShouldThrowExcptionIfContentNotCurrentlyStored()
        {
            // Arrange
            var mcp = new MessageContextProvider();
            var message = new Mock<IMessage<object>>();
            message.Setup(mc => mc.Content).Returns(new object());

            // Act
            Assert.Throws<MessageContextException>(() => mcp.ReleaseMessageContext(message.Object));
        }

        [Test]
        public void Release_ValidMessageContent_ShouldThrowExcptionIfContentIsReleasedMoreThanOnce()
        {
            // Arrange
            var mcp = new MessageContextProvider();
            var message = new Mock<IMessage<object>>();
            message.Setup(mc => mc.Content).Returns(new object());
            mcp.StoreMessageContext(message.Object);

            // Act
            mcp.ReleaseMessageContext(message.Object);
            Assert.Throws<MessageContextException>(() => mcp.ReleaseMessageContext(message.Object));
        }
    }
}
