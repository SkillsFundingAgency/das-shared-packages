using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.TrackProgress.Messages.Events;

namespace SFA.DAS.NServiceBus.AzureFunctions.Extensions.Example.Handlers;

public class NewProgressAddedEventHandler : IHandleMessages<NewProgressAddedEvent>
{
    public async Task Handle(NewProgressAddedEvent message, IMessageHandlerContext context)
    {
        Console.WriteLine("Processing NewProgressAddedEvent Message {0}", message?.CommitmentsApprenticeshipId);
    }
}