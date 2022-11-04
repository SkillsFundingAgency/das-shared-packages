using System.Security.Cryptography.X509Certificates;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus.Pipeline;
using NServiceBus.Testing;
using NServiceBus.Unicast.Messages;
using NUnit.Framework;
using SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests.Data;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests;
public class WhenLoggingIncomingBehaviour
{
    [Test]
    public async Task Should_log_the_information()
    {
        var logger = new FakeLogger();
        var loggerFactorMock = new Mock<ILoggerFactory>();
        loggerFactorMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger);

        var behavior = new LogIncomingBehaviour(loggerFactorMock.Object);
        var context = new TestableIncomingLogicalMessageContext
        {
            Message = new LogicalMessage(new MessageMetadata(typeof(TestEvent)), new TestEvent())
        };

        await behavior.Invoke(context, (c) => Task.CompletedTask)
            .ConfigureAwait(false);

        logger.LogInformationCallCount.Should().Be(1);
        logger.InformationMessages.First().Should().StartWith("Received message");
    }
}