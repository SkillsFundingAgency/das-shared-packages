using Microsoft.Extensions.Logging;

namespace SFA.DAS.Http.Logging
{
    internal static class EventIds
    {
        public static readonly EventId SendingRequest = new EventId(1, nameof(SendingRequest));
        public static readonly EventId ReceivedResponse = new EventId(2, nameof(ReceivedResponse));
    }
}