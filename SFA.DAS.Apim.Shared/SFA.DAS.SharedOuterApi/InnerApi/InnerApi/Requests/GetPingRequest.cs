using SFA.DAS.SharedOuterApi.Interfaces;

namespace SFA.DAS.SharedOuterApi.InnerApi.InnerApi.Requests
{
    public class GetPingRequest : IGetApiRequest
    {
        public string GetUrl => "ping";
    }
}