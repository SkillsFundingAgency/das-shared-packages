using Microsoft.Extensions.Logging;

namespace SFA.DAS.Http.Logging
{
    internal static class EventIds
    {
        public static readonly EventId SendingRequest = new EventId(1, nameof(SendingRequest));
        public static readonly EventId SendingRequestHeaders = new EventId(2, nameof(SendingRequestHeaders));
        public static readonly EventId ReceivedResponse = new EventId(3, nameof(ReceivedResponse));
        public static readonly EventId ReceivedResponseHeaders = new EventId(4, nameof(ReceivedResponseHeaders));
    }
}