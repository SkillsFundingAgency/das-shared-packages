using SFA.DAS.ApiSubstitute.WebAPI;


namespace SFA.DAS.ProviderEventsApiSubstitute.WebAPI
{
    public class ProviderEventsApi : WebApiSubstitute
    {
        public string BaseAddress { get; private set; }

        public ProviderEventsApiMessageHandler ProviderEventsApiMessageHandler { get; private set; }

        public ProviderEventsApi(ProviderEventsApiMessageHandler apiMessageHandler) : base(apiMessageHandler)
        {
            BaseAddress = apiMessageHandler.BaseAddress;
            ProviderEventsApiMessageHandler = apiMessageHandler;
        }

    }
}
