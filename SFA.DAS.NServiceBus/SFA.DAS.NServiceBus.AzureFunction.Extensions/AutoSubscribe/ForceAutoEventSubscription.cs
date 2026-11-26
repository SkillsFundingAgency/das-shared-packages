using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions.AutoSubscribe;

/*
 * 
NOTE: These clases are added via the Nuget Package SFA.DAS.NServiceBus.AzureFunction.Extension so that we can automatically register the Subscription rules.
These will only be registered in NServiceBus when a message is processed, so this sends a local message as a work round (until this issue is resolved)
 */

public class ForceAutoEventSubscription : IMessage { }

public class ForceAutoEventSubscriptionFunction
{
    private readonly IEndpointInstance functionEndpoint;

    public ForceAutoEventSubscriptionFunction(IEndpointInstance functionEndpoint)
        => this.functionEndpoint = functionEndpoint;

    [Function("ForceAutoSubscriptionFunction")]
    public async Task Run(
        [TimerTrigger("* * * 1 1 *", RunOnStartup = true)] TimerInfo myTimer,
        ILogger logger, ExecutionContext executionContext)
    {
        await functionEndpoint.Send(new ForceAutoEventSubscription());
    }
}

public class ForceAutoEventSubscriptionHandler : IHandleMessages<ForceAutoEventSubscription>
{
    public Task Handle(ForceAutoEventSubscription message, IMessageHandlerContext context)
        => Task.CompletedTask;
}