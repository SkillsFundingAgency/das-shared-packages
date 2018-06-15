using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging.AzureServiceBus.UnitTests
{
    [TestFixture]
    public class AzureServiceBusMessageTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void CompleteAsync_KeepAlive_ShouldNotThrowNullException(bool keepalive)
        {
            var msg = CreateAzureServiceBusMessage(keepalive);
            Assert.ThrowsAsync<InvalidOperationException>(() => msg.AbortAsync());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void AbortAsync_KeepAlive_ShouldNotThrowNullException(bool keepalive)
        {
            var msg = CreateAzureServiceBusMessage(keepalive);

            Assert.ThrowsAsync<InvalidOperationException>(() => msg.AbortAsync());
        }

        private AzureServiceBusMessage<object> CreateAzureServiceBusMessage(bool keepAlive)
        {
            var log = new Mock<ILog>();
            return new AzureServiceBusMessage<object>(new BrokeredMessage(), log.Object, keepAlive);
        }
    }
}
