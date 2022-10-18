using AutoFixture.NUnit3;
using NServiceBus.Pipeline;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests.Data;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests;
public class WhenLoggingOutgoingBehaviour
{
    [Test]
    public async Task Should_not_fail_when_called()
    {
        var behavior = new LogOutgoingBehaviour();
        var context = new TestableOutgoingLogicalMessageContext
        {
            Message = new OutgoingLogicalMessage(typeof(TestEvent), new TestEvent())
        };

        await behavior.Invoke(context, _ => Task.CompletedTask)
            .ConfigureAwait(false);
    }
}