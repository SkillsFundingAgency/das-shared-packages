using System;

namespace SFA.DAS.Events.Dispatcher
{
    public class HandlerNotRegisteredException : Exception
    {
        internal HandlerNotRegisteredException(Type eventType, string eventName)
        {
            EventType = eventType;
            EventName = eventName;
        }

        public Type EventType { get; }

        public string EventName { get; }
    }
}
