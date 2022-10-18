using AutoFixture.NUnit3;
using NServiceBus.Pipeline;
using NServiceBus.Testing;
using NServiceBus.Unicast.Messages;
using NUnit.Framework;
using SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests.Data;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests;
public class WhenLoggingIncomingBehaviour
{
    [Test]
    public async Task Should_not_fail_when_called()
    {
        var behavior = new LogIncomingBehaviour();
        var context = new TestableIncomingLogicalMessageContext
        {
            Message = new LogicalMessage(new MessageMetadata(typeof(TestEvent)), new TestEvent())
        };

        await behavior.Invoke(context, (c) => Task.CompletedTask)
            .ConfigureAwait(false);
    }
}