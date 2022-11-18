using NServiceBus;
using SFA.DAS.TrackProgress.Messages.Commands;
using System.Threading.Tasks;
using System;

namespace SFA.DAS.NServiceBus.AzureFunctions.Extensions.Example.Handlers;

public class CacheKsbsCommandHandler : IHandleMessages<CacheKsbsCommand>
{
    public async Task Handle(CacheKsbsCommand message, IMessageHandlerContext context)
    {
        Console.WriteLine("Processing Cache KSB Command for {0}", message?.StandardUid);
    }
}