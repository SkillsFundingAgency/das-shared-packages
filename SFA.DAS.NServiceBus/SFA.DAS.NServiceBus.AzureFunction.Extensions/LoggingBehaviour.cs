using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus.Pipeline;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions
{
    public class LogIncomingBehaviour : IBehavior<IIncomingLogicalMessageContext, IIncomingLogicalMessageContext>
    {
        private readonly ILogger _logger;

        public LogIncomingBehaviour()
        {
            _logger = LoggerFactory.Create(b => b.Services.AddLogging(c=>c.AddConsole())).CreateLogger<LogIncomingBehaviour>();
        }

        public async Task Invoke(IIncomingLogicalMessageContext context, Func<IIncomingLogicalMessageContext, Task> next)
        {
            context.MessageHeaders.TryGetValue("NServiceBus.MessageIntent", out var intent);
            var types = context.Message.MessageType.Name;
            _logger.LogInformation($"Received message {context.MessageId} (`{types}` intent `{intent}`)");

            await next(context);
        }
    }

    public class LogOutgoingBehaviour : IBehavior<IOutgoingLogicalMessageContext, IOutgoingLogicalMessageContext>
    {
        private readonly ILogger _logger;

        public LogOutgoingBehaviour()
        {
            _logger = LoggerFactory.Create(b => b.Services.AddLogging(c => c.AddConsole())).CreateLogger<LogOutgoingBehaviour>();
        }

        public async Task Invoke(IOutgoingLogicalMessageContext context, Func<IOutgoingLogicalMessageContext, Task> next)
        {
            var types = context.Message.MessageType.Name;
            _logger.LogInformation($"Sending message {context.MessageId} (`{types}`)");

            await next(context);
        }
    }
}
