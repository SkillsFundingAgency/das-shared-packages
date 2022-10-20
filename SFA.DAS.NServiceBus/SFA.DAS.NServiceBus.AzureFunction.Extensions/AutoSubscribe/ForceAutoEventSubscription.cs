using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions.AutoSubscribe;

/*
 * 
NOTE: These clases are added via the Nuget Package SFA.DAS.NServiceBus.AzureFunction.Extension so that we can automatically register the Subscription rules.
These will only be registered in NServiceBus when a message is processed, so this sends a local message as a work round
 
 */

public class ForceAutoEventSubscription : IMessage { }

public class ForceAutoEventSubscriptionFunction
{
    private readonly IFunctionEndpoint functionEndpoint;

    public ForceAutoEventSubscriptionFunction(IFunctionEndpoint functionEndpoint)
        => this.functionEndpoint = functionEndpoint;

    [FunctionName("ForceAutoSubscriptionFunction")]
    public async Task Run(
        [TimerTrigger("* * * 1 1 *", RunOnStartup = true)] TimerInfo myTimer,
        ILogger logger, ExecutionContext executionContext)
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