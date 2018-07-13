using System;
using NServiceBus;

namespace SFA.DAS.NServiceBus
{
    public abstract class Event : IEvent
    {
        public DateTime Created { get; set; }
    }
}