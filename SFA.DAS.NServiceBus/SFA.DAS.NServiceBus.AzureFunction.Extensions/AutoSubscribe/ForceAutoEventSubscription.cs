using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions.AutoSubscribe;

public class ForceAutoEventSubscription : IMessage { }

public class ForceAutoEventSubscriptionFunction
{
    private readonly IFunctionEndpoint functionEndpoint;

    public ForceAutoEventSubscriptionFunction(IFunctionEndpoint functionEndpoint)
        => this.functionEndpoint = functionEndpoint;

    [FunctionName("ForceAutoSubscriptionFunction")]
    public async Task Run(
        [TimerTrigger("* * * 1 1 *", RunOnStartup = true)] TimerInfo myTimer,
        ILogger logger, Microsoft.Azure.WebJobs.ExecutionContext executionContext)
    {
        var sendOptions = SendLocally.Options;
        sendOptions.SetHeader(Headers.ControlMessageHeader, bool.TrueString);
        sendOptions.SetHeader(Headers.MessageIntent, nameof(MessageIntentEnum.Send));
        await functionEndpoint.Send(new ForceAutoEventSubscription(), sendOptions, executionContext, logger);
    }
}

public class ForceAutoEventSubscriptionHandler : IHandleMessages<ForceAutoEventSubscription>
{
    public Task Handle(ForceAutoEventSubscription message, IMessageHandlerContext context)
        => Task.CompletedTask;
}