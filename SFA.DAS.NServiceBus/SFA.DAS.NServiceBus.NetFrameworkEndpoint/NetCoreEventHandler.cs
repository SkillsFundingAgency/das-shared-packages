using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus.NetStandardMessages.Events;

namespace SFA.DAS.NServiceBus.NetFrameworkEndpoint
{
    public class NetCoreEventHandler : IHandleMessages<NetCoreEvent>
    {
        public Task Handle(NetCoreEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Message '{message.GetType().Name}' received with '{nameof(message.Data)}' property value '{message.Data}'");

            return Task.CompletedTask;
        }
    }
}