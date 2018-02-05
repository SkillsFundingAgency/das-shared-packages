using SFA.DAS.ApiSubstitute.WebAPI;

namespace SFA.DAS.CommitmentsApiSubstitute.WebAPI
{
    public class CommitmentsApi : WebApiSubstitute
    {
        public string BaseAddress { get; private set; }

        public CommitmentsApiMessageHandler CommitmentsApiMessageHandler { get; private set; }

        public CommitmentsApi(CommitmentsApiMessageHandler apiMessageHandler) : base(apiMessageHandler)
        {
            BaseAddress = apiMessageHandler.BaseAddress;
            CommitmentsApiMessageHandler = apiMessageHandler;
        }
    }
}
