using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging.UnitTests.MessageProcessorTests
{
    [TestFixture]
    public class WhenAMessageIsReceived
    {
        public class MessageType
        {
        }

        internal class TestMessageProcessor : MessageProcessor<MessageType>
        {
            internal TestMessageProcessor(IMessageSubscriberFactory messageSubscriberFactory, ILog logger, IMessageContextProvider messageContextProvider) : base(messageSubscriberFactory, logger, messageContextProvider)
            {
            }

            internal bool MessageProcessed;
            internal bool ThrowException;
            internal bool ExceptionHandled;

            public Action<MessageType> OnBeingProcessed;

            protected override async Task ProcessMessage(MessageType messageContent)
            {
                OnBeingProcessed?.Invoke(messageContent);

                if (ThrowException)
                {
                    throw new Exception();
                }

                await Task.Run(() => { MessageProcessed = true; });
            }

            protected override async Task OnErrorAsync(IMessage<MessageType> message, Exception exception)
            {
                await Task.Run(() => { return ExceptionHandled = true; });
            }
        }

        private Mock<IMessageSubscriberFactory> _messageSubscriberFactory;
        private Mock<IMessageSubscriber<MessageType>> _messageSubscriber;
        private Mock<IMessageContextProvider> _messageContextProvider;
        private Mock<ILog> _logger;
        private TestMessageProcessor _messageProcessor;

        [SetUp]
        public void Arrange()
        {
            _messageSubscriber = new Mock<IMessageSubscriber<MessageType>>();
            _messageSubscriberFactory = new Mock<IMessageSubscriberFactory>();
            _messageContextProvider = new Mock<IMessageContextProvider>();
            _logger = new Mock<ILog>();

            _messageSubscriberFactory.Setup(x => x.GetSubscriber<MessageType>()).Returns(_messageSubscriber.Object);

            _messageProcessor = new TestMessageProcessor(_messageSubscriberFactory.Object, _logger.Object, _messageContextProvider.Object);
        }

        [Test]
        public void AndTheMessageIsNullThenNoProcessingIsDone()
        {
            _messageSubscriber.Setup(x => x.ReceiveAsAsync()).ReturnsAsync(null);

            ProcessMessage(() => !_messageProcessor.MessageProcessed);
            
            Assert.IsFalse(_messageProcessor.MessageProcessed);
        }

        [Test]
        public void AndTheMessageHasNoContentThenNoProcessingIsDoneAndTheMessageIsCompleted()
        {
            var message = new Mock<IMessage<MessageType>>();
            _messageSubscriber.Setup(x => x.ReceiveAsAsync()).ReturnsAsync(message.Object);

            ProcessMessage(() => !_messageProcessor.MessageProcessed);

            Assert.IsFalse(_messageProcessor.MessageProcessed);
            message.Verify(x => x.CompleteAsync());
        }

        [Test]
        public void ThenTheMessageIsProcessedAndCompleted()
        {
            var message = new Mock<IMessage<MessageType>>();
            message.SetupGet(x => x.Content).Returns(new MessageType());

            _messageSubscriber.Setup(x => x.ReceiveAsAsync()).ReturnsAsync(message.Object);

            ProcessMessage(() => _messageProcessor.MessageProcessed);

            Assert.IsTrue(_messageProcessor.MessageProcessed);
            message.Verify(x => x.CompleteAsync());
        }

        [Test]
        public void ThenMessageIsStoredAndReleasedInAContext()
        {
            var message = new Mock<IMessage<MessageType>>();
            message.SetupGet(x => x.Content).Returns(new MessageType());

            _messageSubscriber.Setup(x => x.ReceiveAsAsync()).ReturnsAsync(message.Object);

            ProcessMessage(() => _messageProcessor.MessageProcessed);

            _messageContextProvider.Verify(mcp => mcp.StoreMessageContext(message.Object));
            _messageContextProvider.Verify(mcp => mcp.ReleaseMessageContext(message.Object));
        }

        [Test]
        public void AndAnErrorOccursThenTheErrorIsLoggedAndHandled()
        {
            _messageProcessor.ThrowException = true;
            var message = new Mock<IMessage<MessageType>>();
            message.SetupGet(x => x.Content).Returns(new MessageType());
            _messageSubscriber.Setup(x => x.ReceiveAsAsync()).ReturnsAsync(message.Object);

            ProcessMessage(() => _messageProcessor.ExceptionHandled);

            _logger.Verify(x => x.Error(It.IsAny<Exception>(), $"Failed to process message {typeof(MessageType).FullName}"));

            Assert.IsTrue(_messageProcessor.ExceptionHandled);
            Assert.IsFalse(_messageProcessor.MessageProcessed);
            message.Verify(x => x.CompleteAsync(), Times.Never);
        }

        private void ProcessMessage(Func<bool> condition)
        {
            var resetEvent=  new ManualResetEvent(false);
            var cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => _messageProcessor.RunAsync(cancellationTokenSource), cancellationTokenSource.Token)
                .ContinueWith(task => resetEvent.Set(), cancellationTokenSource.Token);

            var timeout = DateTime.Now.AddSeconds(1);
            while (DateTime.Now <= timeout)
            {
                Thread.Sleep(10);
                if (condition())
                {
                    break;
                }
            }

            cancellationTokenSource.Cancel();
            resetEvent.WaitOne(1000);
        }
    }
}
