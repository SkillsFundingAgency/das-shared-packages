using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus.TestMessages.Events;

namespace SFA.DAS.NServiceBus.TestEndpoint
{
    public class SampleEventHandler : IHandleMessages<SampleEvent>
    {
        public Task Handle(SampleEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Message '{message.GetType().Name}' received with '{nameof(message.Data)}' property value '{message.Data}'");

            return Task.CompletedTask;
        }
    }
}