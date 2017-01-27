using System;

namespace SFA.DAS.Events.Dispatcher
{
    internal class EventRegistration
    {
        internal EventRegistration(Type eventType, string eventName, object handler)
        {
            EventType = eventType;
            EventName = eventName;
            Handler = handler;
        }

        internal Type EventType { get; }

        internal string EventName { get; }

        internal object Handler { get; }
    }
}
