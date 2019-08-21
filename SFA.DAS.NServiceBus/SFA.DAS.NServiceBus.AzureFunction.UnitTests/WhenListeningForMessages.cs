using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Moq;
using NServiceBus.Extensibility;
using NServiceBus.Transport;
using NUnit.Framework;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.UnitTests.NServiceBus
{
    public class WhenListeningForMessages
    {
        private Mock<ITriggeredFunctionExecutor> _executor;
        private NServiceBusTriggerAttribute _attribute;
        private TestListener _listener;
        private MessageContext _messageContext;
        private ParameterInfo _parameter;

        [SetUp]
        public void Arrange()
        {
            _executor = new Mock<ITriggeredFunctionExecutor>();
            _attribute = new NServiceBusTriggerAttribute();
            _listener = new TestListener(_executor.Object, _attribute, _parameter);
            _messageContext = new MessageContext("1", new Dictionary<string, string>(), new byte[]{1,2,3}, new TransportTransaction(), new CancellationTokenSource(), new ContextBag());

            _executor.Setup(e => e.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new FunctionResult(true));
        }

        [Test]
        public async Task ThenCallsBindingWhenMessageReceived()
        {
            //Act
            await _listener.CallOnMessage(_messageContext, Mock.Of<IDispatchMessages>());

            //Assert
            _executor.Verify(expression => expression.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void ThenIfCallToBindingFailsThrowsException()
        {
            //Arrange
            _executor.Setup(e => e.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new FunctionResult(false, new Exception()));

            //Act + Assert
            Assert.ThrowsAsync<Exception>(() => _listener.CallOnMessage(_messageContext, Mock.Of<IDispatchMessages>()));
        }

        private class TestListener : NServiceBusListener
        {
            public TestListener(ITriggeredFunctionExecutor contextExecutor, NServiceBusTriggerAttribute attribute, ParameterInfo parameter ) : base(contextExecutor, attribute, parameter)
            {
            }

            public async Task CallOnMessage(MessageContext context, IDispatchMessages dispatcher)
            {
                await OnMessage(context, dispatcher);
            }
        }
    }
}
