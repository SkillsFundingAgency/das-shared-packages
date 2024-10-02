﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.WebJobs.Host.Executors;
using Moq;
using NServiceBus.Extensibility;
using NServiceBus.Transport;
using NUnit.Framework;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using SFA.DAS.NServiceBus.AzureFunction.Configuration;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;

namespace SFA.DAS.NServiceBus.AzureFunction.UnitTests.Hosting
{
    public class WhenListeningForMessages
    {
        private Mock<ITriggeredFunctionExecutor> _executor;
        private NServiceBusTriggerAttribute _attribute;
        private TestListener _listener;
        private MessageContext _messageContext;
        private NServiceBusOptions _options;

        [SetUp]
        public void Arrange()
        {
            _executor = new Mock<ITriggeredFunctionExecutor>();
            _attribute = new NServiceBusTriggerAttribute();
            _options = new NServiceBusOptions();
            _listener = new TestListener(_executor.Object, _attribute, null, _options);
            _messageContext = new MessageContext("1", new Dictionary<string, string>(), new byte[]{1,2,3}, new TransportTransaction(), "testReceiveAddress", new ContextBag());

            _executor.Setup(e => e.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new FunctionResult(true));
        }

        [Test]
        public async Task ThenCallsBindingWhenMessageReceived()
        {
            //Act
            await _listener.CallOnMessage(_messageContext, Mock.Of<IMessageDispatcher>(), CancellationToken.None);

            //Assert
            _executor.Verify(expression => expression.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenInvokesOnMessageReceivedWhenOptionIsConfigured()
        {
            //Arrange
            bool onMessageReceivedCalled = false;
            MessageContext messageContext = null;
            _options.OnMessageReceived = o =>
            {
                onMessageReceivedCalled = true;
                messageContext = o;
            };

            //Act
            await _listener.CallOnMessage(_messageContext, Mock.Of<IMessageDispatcher>(), CancellationToken.None);

            //Assert
            onMessageReceivedCalled.Should().Be(true);
            messageContext.Should().Be(_messageContext);
        }

        [Test]
        public async Task ThenInvokesOnMessageProcessedWhenOptionIsConfigured()
        {
            //Arrange
            bool onMessageProcessedCalled = false;
            MessageContext messageContext = null;
            _options.OnMessageProcessed= o =>
            {
                onMessageProcessedCalled = true;
                messageContext = o;
            };

            //Act
            await _listener.CallOnMessage(_messageContext, Mock.Of<IMessageDispatcher>(), CancellationToken.None);

            //Assert
            onMessageProcessedCalled.Should().Be(true);
            messageContext.Should().Be(_messageContext);
        }

        [Test]
        public async Task ThenInvokesOnMessageErroredWhenOptionIsConfigured()
        {
            //Arrange
            bool onMessageErroredCalled = false;
            MessageContext messageContext = null;
            Exception expectedException = null;
            Exception testException = new Exception("Test");
            _options.OnMessageErrored = (e, o) =>
            {
                onMessageErroredCalled = true;
                messageContext = o;
                expectedException = e;
            };

            _executor.Setup(e => e.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new FunctionResult(false, testException));

            //Act
            try
            {
                await _listener.CallOnMessage(_messageContext, Mock.Of<IMessageDispatcher>(), CancellationToken.None);
            }
            catch
            {
                // blank
            }

            //Assert
            onMessageErroredCalled.Should().Be(true);
            messageContext.Should().Be(_messageContext);
            expectedException.Should().Be(testException);
        }

        [Test]
        public void ThenIfCallToBindingFailsThrowsException()
        {
            //Arrange
            _executor.Setup(e => e.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new FunctionResult(false, new Exception()));

            //Act + Assert
            Assert.ThrowsAsync<Exception>(() => _listener.CallOnMessage(_messageContext, Mock.Of<IMessageDispatcher>(), CancellationToken.None));
        }
        private class TestListener : NServiceBusListener
        {
            public TestListener(ITriggeredFunctionExecutor contextExecutor, NServiceBusTriggerAttribute attribute, ParameterInfo parameter, NServiceBusOptions options ) : base(contextExecutor, attribute, parameter, options)
            {
            }

            public async Task CallOnMessage(MessageContext context, IMessageDispatcher dispatcher, CancellationToken cancellationToken)
            {
                await OnMessage(context, dispatcher, cancellationToken);
            }
        }
    }
}
