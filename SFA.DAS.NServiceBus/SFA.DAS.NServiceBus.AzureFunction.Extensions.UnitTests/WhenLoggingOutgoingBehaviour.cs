using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus.Pipeline;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests.Data;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests;
public class WhenLoggingOutgoingBehaviour
{
    [Test]
    public async Task Should_log_an_information_line()
    {
        var logger = new FakeLogger();
        var loggerFactorMock = new Mock<ILoggerFactory>();
        loggerFactorMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger);

        var behavior = new LogOutgoingBehaviour(loggerFactorMock.Object);
        var context = new TestableOutgoingLogicalMessageContext
        {
            Message = new OutgoingLogicalMessage(typeof(TestEvent), new TestEvent())
        };

        await behavior.Invoke(context, _ => Task.CompletedTask)
            .ConfigureAwait(false);

        logger.LogInformationCallCount.Should().Be(1);
    }
}