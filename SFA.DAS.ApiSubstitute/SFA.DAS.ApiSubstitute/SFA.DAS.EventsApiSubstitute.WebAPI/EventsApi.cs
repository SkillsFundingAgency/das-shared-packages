using SFA.DAS.ApiSubstitute.WebAPI;

namespace SFA.DAS.EventsApiSubstitute.WebAPI
{
    public class EventsApi : WebApiSubstitute
    {
        public string BaseAddress { get; private set; }

        public EventsApiMessageHandler AccountsApiMessageHandler { get; private set; }

        public EventsApi(EventsApiMessageHandler apiMessageHandler) : base(apiMessageHandler)
        {
            BaseAddress = apiMessageHandler.BaseAddress;
            AccountsApiMessageHandler = apiMessageHandler;
        }
    }
}
